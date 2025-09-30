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
                // 1️⃣ Generamos externalReference primero
                var externalReference = Guid.NewGuid().ToString();
                dto.ExternalReference = externalReference;

                // 2️⃣ Creamos preferencia en MP
                var preference = await _paymentService.CreateCheckoutPreferenceAsync(dto);
                if (preference == null)
                    return BadRequest(new { error = "No se pudo generar la preferencia" });

                // 3️⃣ Guardamos la venta en background (no bloqueamos al cliente)
                _ = Task.Run(async () =>
                {
                    try
                    {
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
                            }).ToList(),
                            Total = dto.Items.Sum(i => i.UnitPrice * i.Quantity)
                        };

                        await _ventaRepository.AddAsync(venta);
                        _logger.LogInformation("Venta creada async con Id {VentaId} y ExternalReference {ExternalReference}",
                                                venta.Id, venta.ExternalReference);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "❌ Error guardando venta en background");
                    }
                });

                // 4️⃣ Respondemos rápido al front
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
        public async Task<IActionResult> Webhook()
        {
            try
            {
                string body = await new StreamReader(Request.Body).ReadToEndAsync();
                _logger.LogInformation("📩 Webhook recibido (raw): {Body}", body);

                string? type = null;
                string? dataId = null;

                // 1️⃣ Si viene como JSON
                if (!string.IsNullOrWhiteSpace(body) && body.TrimStart().StartsWith("{"))
                {
                    using var doc = JsonDocument.Parse(body);
                    var root = doc.RootElement;

                    if (root.TryGetProperty("type", out var typeProp))
                        type = typeProp.GetString();
                    else if (root.TryGetProperty("topic", out var topicProp))
                        type = topicProp.GetString();

                    if (root.TryGetProperty("data", out var dataProp) &&
                        dataProp.ValueKind == JsonValueKind.Object &&
                        dataProp.TryGetProperty("id", out var idProp))
                    {
                        dataId = idProp.ValueKind == JsonValueKind.Number
                            ? idProp.GetInt64().ToString()
                            : idProp.GetString();
                    }
                }
                else
                {
                    // 2️⃣ Si viene como form-urlencoded
                    var form = await Request.ReadFormAsync();
                    type = form["type"].FirstOrDefault() ?? form["topic"].FirstOrDefault();
                    dataId = form["data.id"].FirstOrDefault();
                }

                if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(dataId))
                {
                    _logger.LogWarning("⚠️ Webhook sin 'type' o 'data.id'. Ignorado.");
                    return Ok(); // devolvemos 200 para que MP no reintente infinito
                }

                _logger.LogInformation("✅ Webhook parseado correctamente. Type={Type}, PaymentId={PaymentId}", type, dataId);

                if (type != "payment")
                {
                    _logger.LogInformation("ℹ️ Evento ignorado: {Type}", type);
                    return Ok();
                }

                // 🔍 Consultamos el pago en MercadoPago
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);

                var response = await client.GetAsync($"https://api.mercadopago.com/v1/payments/{dataId}");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("❌ Error al consultar pago en MP: {Status}", response.StatusCode);
                    return StatusCode(500, "Error al consultar Mercado Pago");
                }

                var json = await response.Content.ReadAsStringAsync();
                using var paymentDoc = JsonDocument.Parse(json);
                var rootPayment = paymentDoc.RootElement;

                var status = rootPayment.TryGetProperty("status", out var statusProp)
                    ? statusProp.GetString()
                    : null;

                var externalReference = rootPayment.TryGetProperty("external_reference", out var refProp)
                    ? refProp.GetString()
                    : null;

                if (string.IsNullOrEmpty(status) || string.IsNullOrEmpty(externalReference))
                {
                    _logger.LogWarning("⚠️ Pago sin status o external_reference");
                    return Ok();
                }

                _logger.LogInformation("💰 Pago recibido. Status={Status}, ExternalRef={ExternalReference}", status, externalReference);

                // TODO: acá mantenés tu lógica de actualizar venta, descontar stock y enviar mail
                // ...

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
