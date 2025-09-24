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
                // Crear la venta en la base con estado Pendiente
                var venta = new Venta
                {
                    Date = DateTime.UtcNow,
                    Status = VentaStatus.Pendiente,
                    ExternalReference = Guid.NewGuid().ToString(),
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

                // Asociar ese ExternalReference a MercadoPago
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
                return BadRequest();

            try
            {
                // 🔹 Verificar que las variables de entorno estén cargadas
                var webhookSecret = configuration["MercadoPago:WebhookSecret"];
                _logger.LogInformation("AccessToken cargado: {AccessTokenLoaded}",
                    string.IsNullOrEmpty(_accessToken) ? "NO CARGADO" : "CARGADO");
                _logger.LogInformation("WebhookSecret cargado: {WebhookSecretLoaded}",
                    string.IsNullOrEmpty(webhookSecret) ? "NO CARGADO" : "CARGADO");

                _logger.LogInformation("Webhook recibido: {Notification}", notification.ToString());

                if (!notification.TryGetProperty("type", out var typeProp) ||
                    !notification.TryGetProperty("data", out var dataProp) ||
                    !dataProp.TryGetProperty("id", out var idProp))
                {
                    _logger.LogWarning("Notificación inválida: {Notification}", notification.ToString());
                    return BadRequest();
                }

                var type = typeProp.GetString();
                var id = idProp.GetString();

                if (type == "payment" && !string.IsNullOrEmpty(id))
                {
                    var httpClient = _httpClientFactory.CreateClient();
                    var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.mercadopago.com/v1/payments/{id}");
                    request.Headers.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);

                    var response = await httpClient.SendAsync(request);
                    var json = await response.Content.ReadAsStringAsync();
                    var paymentInfo = JsonSerializer.Deserialize<MercadoPagoPaymentDto>(json);

                    if (paymentInfo == null)
                    {
                        _logger.LogWarning("No se pudo deserializar paymentInfo");
                        return BadRequest();
                    }

                    var venta = await _ventaRepository.GetByExternalReferenceAsync(paymentInfo.ExternalReference);

                    if (venta != null && paymentInfo.Status == "approved" && venta.Status == VentaStatus.Pendiente)
                    {
                        venta.Status = VentaStatus.Enviado; // o Pagado
                        venta.PaymentId = paymentInfo.Id.ToString();
                        venta.PaymentStatus = paymentInfo.Status;
                        venta.StatusDetail = paymentInfo.StatusDetail;
                        venta.TransactionAmount = paymentInfo.TransactionAmount;

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

                        // Enviar correo de confirmación de compra
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

