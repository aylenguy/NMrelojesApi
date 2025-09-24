using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Application.Interfaces;
using Application.Model.Request;
using Application.Model;
using Domain.Entities;
using Domain.Interfaces;


namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<PaymentController> _logger;
        private readonly IVentaRepository _ventaRepository;
        private readonly IProductRepository _productRepository;
        private readonly string _accessToken;
        private readonly EmailService _emailService;            

        public PaymentController(
            IPaymentService paymentService,
            IHttpClientFactory httpClientFactory,
            ILogger<PaymentController> logger,
            IConfiguration configuration,
            IVentaRepository ventaRepository,
            IProductRepository productRepository,
            EmailService emailService
        )
        {
            _paymentService = paymentService;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _accessToken = configuration["MercadoPago:AccessToken"];
            _ventaRepository = ventaRepository;
            _productRepository = productRepository;
            _emailService = emailService;
        }

        [HttpPost("create-payment")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentRequestDto dto)
        {
            var paymentResult = await _paymentService.CreatePaymentAsync(dto);
            return Ok(paymentResult);
        }

        [HttpPost("create-checkout")]
        public async Task<IActionResult> CreateCheckout([FromBody] CheckoutRequestDto dto)
        {
            try
            {
                // 1️⃣ Validar productos
                foreach (var item in dto.Items)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);
                    if (product == null)
                        return BadRequest(new { error = $"El producto con Id {item.ProductId} no existe" });
                }

                // 2️⃣ Crear la venta en estado Pendiente
                var externalReference = Guid.NewGuid().ToString();

                var venta = new Venta
                {
                    Date = DateTime.UtcNow,
                    Status = VentaStatus.Pendiente,
                    ExternalReference = externalReference,
                    CustomerEmail = dto.PayerEmail ?? string.Empty,
                    DetalleVentas = dto.Items.Select(i => new DetalleVenta
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        Subtotal = i.UnitPrice * i.Quantity
                    }).ToList()
                };

                venta.Total = venta.DetalleVentas.Sum(d => d.Subtotal);

                await _ventaRepository.AddAsync(venta);
                _logger.LogInformation("Venta creada con Id {VentaId} y ExternalReference {ExternalReference}",
                                        venta.Id, venta.ExternalReference);

                // 3️⃣ Asociar ExternalReference a MercadoPago
                dto.ExternalReference = externalReference;

                // 4️⃣ Crear preferencia en MercadoPago
                var preference = await _paymentService.CreateCheckoutPreferenceAsync(dto);
                if (preference == null)
                    return BadRequest(new { error = "No se pudo generar la preferencia" });

                // 🔹 5️⃣ Devolver preferencia + externalReference al front
                return Ok(new
                {
                    preference,
                    externalReference
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en create-checkout");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] JsonElement notification)
        {
            if (notification.ValueKind == JsonValueKind.Undefined || notification.ValueKind == JsonValueKind.Null)
                return BadRequest();

            try
            {
                _logger.LogInformation("Webhook recibido: {Notification}", notification.ToString());

                // 🔹 Intentamos obtener 'type' y 'data.id' de manera segura
                if (!notification.TryGetProperty("type", out var typeProp) ||
                    !notification.TryGetProperty("data", out var dataProp) ||
                    !dataProp.TryGetProperty("id", out var idProp))
                {
                    _logger.LogWarning("Notificación inválida o incompleta: {Notification}", notification.ToString());
                    return Ok(); // Devolvemos OK para no generar reintentos
                }

                string? type = typeProp.GetString();
                string? id = idProp.GetString();

                if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(id))
                {
                    _logger.LogWarning("Notificación con valores nulos: {Notification}", notification.ToString());
                    return Ok();
                }

                if (type == "payment")
                {
                    var httpClient = _httpClientFactory.CreateClient();
                    var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.mercadopago.com/v1/payments/{id}");
                    request.Headers.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);

                    var response = await httpClient.SendAsync(request);
                    var json = await response.Content.ReadAsStringAsync();
                    var paymentInfo = JsonSerializer.Deserialize<MercadoPagoPaymentDto>(json);

                    if (paymentInfo != null)
                    {
                        var venta = await _ventaRepository.GetByExternalReferenceAsync(paymentInfo.ExternalReference);

                        if (venta != null && paymentInfo.Status == "approved" && venta.Status == VentaStatus.Pendiente)
                        {
                            // 🔹 Actualizar estado de venta
                            venta.Status = VentaStatus.Pagado;

                            foreach (var detalle in venta.DetalleVentas)
                            {
                                var product = await _productRepository.GetByIdAsync(detalle.ProductId);
                                if (product != null)
                                {
                                    product.Stock -= detalle.Quantity;
                                    await _productRepository.UpdateAsync(product);
                                }
                            }

                            await _ventaRepository.UpdateAsync(venta);
                            _logger.LogInformation("Venta actualizada a Enviado: {VentaId}", venta.Id);

                            // 🔹 Enviar correo de confirmación de compra
                            if (!string.IsNullOrEmpty(venta.CustomerEmail))
                            {
                                var productosCorreo = new List<(string nombreProducto, int cantidad, decimal precio)>();
                                foreach (var detalle in venta.DetalleVentas)
                                {
                                    var product = await _productRepository.GetByIdAsync(detalle.ProductId);
                                    productosCorreo.Add((
                                        nombreProducto: product?.Name ?? "Producto",
                                        cantidad: detalle.Quantity,
                                        precio: detalle.UnitPrice
                                    ));
                                }

                                _emailService.EnviarCorreoConfirmacionCompra(
                                    venta.CustomerEmail,
                                    venta.ExternalReference,
                                    productosCorreo,
                                    venta.Total
                                );

                                _logger.LogInformation("Correo de confirmación enviado a {Email}", venta.CustomerEmail);
                            }
                        }
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando webhook de Mercado Pago");
                return StatusCode(500);
            }
        }



    }
}
