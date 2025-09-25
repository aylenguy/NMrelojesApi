using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Application.Interfaces;
using Application.Model.Request;
using Application.Model;
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

                // 🔹 5️⃣ Devolver preferencia + externalReference al front
                // 🔹 5️⃣ Devolver preferencia + externalReference al front
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
        public async Task<IActionResult> Webhook([FromBody] JsonElement notification)
        {
            try
            {
                _logger.LogInformation("📩 Webhook recibido: {Notification}", notification.ToString());

                // 👇 Capturamos el id de pago desde query o body
                string paymentId = null;
                string eventType = null;

                if (HttpContext.Request.Query.ContainsKey("id"))
                {
                    paymentId = HttpContext.Request.Query["id"].ToString();
                    eventType = HttpContext.Request.Query["type"].ToString();
                }
                else
                {
                    if (notification.TryGetProperty("data", out var data))
                        paymentId = data.GetProperty("id").GetString();
                    if (notification.TryGetProperty("type", out var type))
                        eventType = type.GetString();
                }

                if (string.IsNullOrEmpty(paymentId))
                {
                    _logger.LogError("❌ No se recibió id de pago en webhook");
                    return BadRequest();
                }

                if (eventType != "payment")
                    return Ok(); // ignorar si no es pago

                // 🔹 Consultar el pago en MP
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
                    // actualizar venta, stock y mandar mail (igual que ya tenés)
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en webhook");
                return StatusCode(500, ex.Message);
            }
        }



    }
}
