using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Application.Interfaces;
using Application.Model.Request;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;

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

                return Ok(new
                {
                    initPoint = preference.InitPoint,
                    sandboxInitPoint = preference.SandboxInitPoint,
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
        [AllowAnonymous]
        public async Task<IActionResult> Webhook([FromBody] WebhookDto notification)
        {
            try
            {
                _logger.LogInformation("📩 Webhook recibido: {Notification}", JsonSerializer.Serialize(notification));

                string paymentId = null;
                string eventType = null;

                if (HttpContext.Request.Query.ContainsKey("id"))
                {
                    paymentId = HttpContext.Request.Query["id"].ToString();
                    eventType = HttpContext.Request.Query["type"].ToString();
                }
                else
                {
                    paymentId = notification?.Data?.Id;
                    eventType = notification?.Type;
                }

                if (string.IsNullOrEmpty(paymentId))
                {
                    _logger.LogError("❌ No se recibió id de pago en webhook");
                    return BadRequest();
                }

                if (eventType != "payment")
                {
                    _logger.LogInformation("ℹ️ Evento ignorado porque no es pago: {EventType}", eventType);
                    return Ok();
                }

                // 🔹 Consultar el pago en Mercado Pago
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);

                var response = await client.GetAsync($"https://api.mercadopago.com/v1/payments/{paymentId}");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("❌ Error al consultar pago en MP: {Status}", response.StatusCode);
                    return StatusCode(500);
                }

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                var status = root.GetProperty("status").GetString();
                var externalReference = root.GetProperty("external_reference").GetString();

                _logger.LogInformation("✅ Pago recibido. Status={Status}, ExternalRef={ExternalReference}", status, externalReference);

                var venta = await _ventaRepository.GetByExternalReferenceAsync(externalReference);
                if (venta == null)
                {
                    _logger.LogError("❌ No se encontró venta con referencia {ExternalReference}", externalReference);
                    return NotFound();
                }

                if (status == "approved")
                {
                    _logger.LogInformation("💌 Pago aprobado. Se actualizará la venta, se descontará stock y se enviará el mail a {Email}", venta.CustomerEmail);

                    venta.PaymentStatus = "approved";
                    venta.Status = VentaStatus.Entregado; // o Enviado, según tu flujo
                    await _ventaRepository.UpdateAsync(venta);

                    // 🔹 Descontar stock
                    foreach (var detalle in venta.DetalleVentas)
                    {
                        var product = await _productRepository.GetByIdAsync(detalle.ProductId);
                        if (product != null)
                        {
                            product.Stock -= detalle.Quantity;
                            await _productRepository.UpdateAsync(product);
                        }
                    }

                    // 🔹 Preparar lista de productos para el correo
                    var productos = venta.DetalleVentas
                        .Select(d => (d.Product?.Name ?? $"Producto {d.ProductId}", d.Quantity, d.UnitPrice))
                        .ToList();

                    // 🔹 Enviar mail de confirmación de compra
                    _emailService.EnviarCorreoConfirmacionCompra(
                        venta.CustomerEmail,
                        venta.Id.ToString(),
                        productos,
                        venta.Total
                    );
                }
                else
                {
                    _logger.LogWarning("⚠️ Pago no aprobado. Status={Status}", status);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en webhook");
                return StatusCode(500, ex.Message);
            }
        }

        public class WebhookDto
        {
            public string Type { get; set; }
            public WebhookData Data { get; set; }
        }

        public class WebhookData
        {
            public string Id { get; set; }
        }

    }
}
