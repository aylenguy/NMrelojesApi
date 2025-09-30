using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Application.Interfaces;
using Application.Model.Request;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Application.Services;

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
                var externalReference = Guid.NewGuid().ToString();
                dto.ExternalReference = externalReference;

                var preference = await _paymentService.CreateCheckoutPreferenceAsync(dto);
                if (preference == null)
                    return BadRequest(new { error = "No se pudo generar la preferencia" });

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
                    var form = await Request.ReadFormAsync();
                    type = form["type"].FirstOrDefault() ?? form["topic"].FirstOrDefault();
                    dataId = form["data.id"].FirstOrDefault();
                }

                if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(dataId))
                {
                    _logger.LogWarning("⚠️ Webhook sin 'type' o 'data.id'. Ignorado.");
                    return Ok();
                }

                _logger.LogInformation("✅ Webhook parseado correctamente. Type={Type}, PaymentId={PaymentId}", type, dataId);

                if (type != "payment")
                {
                    _logger.LogInformation("ℹ️ Evento ignorado: {Type}", type);
                    return Ok();
                }

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

                var venta = await _ventaRepository.GetByExternalReferenceAsync(externalReference);
                if (venta == null)
                {
                    _logger.LogWarning("⚠️ No se encontró venta con ExternalReference={ExternalReference}", externalReference);
                    return Ok();
                }

                if (status.Equals("approved", StringComparison.OrdinalIgnoreCase))
                {
                    venta.Status = VentaStatus.Pagado;
                    await _ventaRepository.UpdateAsync(venta);
                    await EnviarCorreoCompra(venta);
                    _logger.LogInformation("✅ Venta {VentaId} actualizada a Pagado", venta.Id);
                }
                else if (status.Equals("rejected", StringComparison.OrdinalIgnoreCase))
                {
                    venta.Status = VentaStatus.Cancelado;
                    await _ventaRepository.UpdateAsync(venta);
                    _logger.LogInformation("❌ Venta {VentaId} cancelada (pago rechazado)", venta.Id);
                }
                else if (status.Equals("in_process", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation("⏳ Pago en proceso para Venta {VentaId}", venta.Id);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en webhook");
                return StatusCode(500, ex.Message);
            }
        }

        private async Task EnviarCorreoCompra(Venta venta)
        {
            try
            {
                var subject = "Confirmación de compra";
                var body = $"Gracias por tu compra. Tu número de pedido es {venta.Id} por un total de {venta.Total:C}.";

                await _emailService.SendEmailAsync(venta.CustomerEmail, subject, body);
                _logger.LogInformation("📧 Correo de confirmación enviado a {Email}", venta.CustomerEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error enviando correo de confirmación");
            }
        }
    }
}

