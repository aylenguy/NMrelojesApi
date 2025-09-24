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
                if (dto.Items == null || !dto.Items.Any())
                    return BadRequest(new { error = "No se enviaron productos." });

                var detalleVentas = new List<DetalleVenta>();
                decimal totalVenta = 0;

                // 🔹 Validar productos y calcular subtotal
                foreach (var item in dto.Items)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);
                    if (product == null)
                        return BadRequest(new { error = $"Producto con Id {item.ProductId} no existe" });

                    if (product.Stock < item.Quantity)
                        return BadRequest(new { error = $"Stock insuficiente para el producto {product.Name}" });

                    var subtotal = product.Price * item.Quantity;
                    detalleVentas.Add(new DetalleVenta
                    {
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price,
                        Subtotal = subtotal
                    });

                    totalVenta += subtotal;

                    // 🔹 Descontar stock inmediatamente
                    product.Stock -= item.Quantity;
                    await _productRepository.UpdateAsync(product);
                }

                // 🔹 Crear la venta
                var venta = new Venta
                {
                    Date = DateTime.UtcNow,
                    Status = VentaStatus.Pendiente,
                    ExternalReference = Guid.NewGuid().ToString(),
                    CustomerEmail = dto.PayerEmail ?? string.Empty,
                    DetalleVentas = detalleVentas,
                    Total = totalVenta
                };

                await _ventaRepository.AddAsync(venta);

                // 🔹 Crear preferencia de Mercado Pago
                dto.ExternalReference = venta.ExternalReference;
                var preference = await _paymentService.CreateCheckoutPreferenceAsync(dto);

                if (preference == null)
                    return BadRequest(new { error = "No se pudo generar la preferencia" });

                return Ok(preference);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en create-checkout");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] JsonElement notification, [FromServices] IConfiguration configuration)
        {
            if (notification.ValueKind == JsonValueKind.Undefined || notification.ValueKind == JsonValueKind.Null)
                return Ok(); // MP siempre recibe 200 para no reintentar

            try
            {
                var webhookSecret = configuration["MercadoPago:WebhookSecret"];
                _logger.LogInformation("AccessToken cargado: {AccessTokenLoaded}",
                    string.IsNullOrEmpty(_accessToken) ? "NO CARGADO" : "CARGADO");
                _logger.LogInformation("WebhookSecret cargado: {WebhookSecretLoaded}",
                    string.IsNullOrEmpty(webhookSecret) ? "NO CARGADO" : "CARGADO");

                _logger.LogInformation("Webhook recibido: {Notification}", notification.ToString());

                string type = null;
                string id = null;

                try
                {
                    type = notification.GetProperty("type").GetString();
                    id = notification.GetProperty("data").GetProperty("id").GetString();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al leer la notificación de MP: {Notification}", notification.ToString());
                    return Ok(); // devolvemos 200 aunque no se pueda procesar
                }

                if (type == "payment" && !string.IsNullOrEmpty(id))
                {
                    var httpClient = _httpClientFactory.CreateClient();
                    var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.mercadopago.com/v1/payments/{id}");
                    request.Headers.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);

                    var response = await httpClient.SendAsync(request);
                    var json = await response.Content.ReadAsStringAsync();

                    _logger.LogInformation("Respuesta de MP para payment {PaymentId}: {Json}", id, json);

                    var paymentInfo = JsonSerializer.Deserialize<MercadoPagoPaymentDto>(json);

                    if (paymentInfo == null)
                    {
                        _logger.LogWarning("No se pudo deserializar paymentInfo");
                        return Ok(); // devolvemos 200 igual
                    }

                    var venta = await _ventaRepository.GetByExternalReferenceAsync(paymentInfo.ExternalReference);

                    if (venta != null && paymentInfo.Status == "approved" && venta.Status == VentaStatus.Pendiente)
                    {
                        // 🔹 Solo actualizar estado y datos de pago
                        venta.Status = VentaStatus.Pagado;
                        venta.PaymentId = paymentInfo.Id.ToString();
                        venta.PaymentStatus = paymentInfo.Status;
                        venta.StatusDetail = paymentInfo.StatusDetail;
                        venta.TransactionAmount = paymentInfo.TransactionAmount;

                        await _ventaRepository.UpdateAsync(venta);

                        // 🔹 Enviar correo de confirmación
                        var productos = venta.DetalleVentas
                            .Select(d => (d.Product.Name, d.Quantity, d.UnitPrice))
                            .ToList();

                        _emailService.EnviarCorreoConfirmacionCompra(
                            venta.CustomerEmail,
                            venta.Id.ToString(),
                            productos,
                            venta.Total
                        );

                        _logger.LogInformation("Venta {VentaId} confirmada y correo enviado", venta.Id);
                    }
                    else
                    {
                        _logger.LogInformation("Pago {PaymentId} recibido con estado {Status}, no se actualizó la venta", id, paymentInfo.Status);
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando webhook de Mercado Pago");
                return Ok(); // devolvemos 200 para que MP no reintente infinito
            }
        }


    }
}

