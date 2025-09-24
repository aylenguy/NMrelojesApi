using MercadoPago.Config;
using MercadoPago.Client.Preference;
using Application.Model.Request;
using Microsoft.Extensions.Configuration;
using Application.Interfaces;
using Application.Model.Response;

public class PaymentServiceSandbox : IPaymentService
{
    private readonly string _accessToken;
    private readonly IConfiguration _configuration;

    public PaymentServiceSandbox(IConfiguration configuration)
    {
        _configuration = configuration;
        _accessToken = _configuration["MercadoPago:AccessToken"];
        if (string.IsNullOrEmpty(_accessToken))
            throw new Exception("No se encontró el AccessToken de MercadoPago en la configuración.");

        MercadoPagoConfig.AccessToken = _accessToken;
    }

    public async Task<CheckoutResponseDto> CreateCheckoutPreferenceAsync(CheckoutRequestDto dto)
    {
        try
        {
            // 🔎 Validaciones mínimas
            if (dto.Items == null || !dto.Items.Any())
                throw new ArgumentException("Debe enviar al menos un ítem para la preferencia.");

            foreach (var item in dto.Items)
            {
                if (string.IsNullOrWhiteSpace(item.Title))
                    throw new ArgumentException("Cada ítem debe tener un título válido.");
                if (item.Quantity <= 0)
                    throw new ArgumentException("La cantidad de cada ítem debe ser mayor a 0.");
                if (item.UnitPrice <= 0)
                    throw new ArgumentException("El precio unitario debe ser mayor a 0.");
            }

            if (string.IsNullOrWhiteSpace(dto.CurrencyId))
                dto.CurrencyId = "ARS"; // 👈 valor por defecto

            if (string.IsNullOrWhiteSpace(dto.PayerEmail))
                throw new ArgumentException("El email del pagador es obligatorio.");

            var request = new PreferenceRequest
            {
                Items = dto.Items.Select(i => new PreferenceItemRequest
                {
                    Title = i.Title,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    CurrencyId = dto.CurrencyId
                }).ToList(),

                Payer = new PreferencePayerRequest
                {
                    Email = dto.PayerEmail
                },

                BackUrls = new PreferenceBackUrlsRequest
                {
                    Success = dto.BackUrls?.Success
                              ?? _configuration["FrontEndUrls:Success"]
                              ?? "https://tusitio.com/success",
                    Failure = dto.BackUrls?.Failure
                              ?? _configuration["FrontEndUrls:Failure"]
                              ?? "https://tusitio.com/failure",
                    Pending = dto.BackUrls?.Pending
                              ?? _configuration["FrontEndUrls:Pending"]
                              ?? "https://tusitio.com/pending"
                },

                AutoReturn = "approved",

                ExternalReference = string.IsNullOrWhiteSpace(dto.ExternalReference)
                                   ? Guid.NewGuid().ToString() // 👈 default si no envía nada
                                   : dto.ExternalReference,

                NotificationUrl = dto.NotificationUrl
                    ?? _configuration["BackEndUrls:NotificationUrl"]
                    ?? "https://tusitio.com/api/payment/notifications"
            };

            var json = System.Text.Json.JsonSerializer.Serialize(request, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });
            Console.WriteLine("[PaymentServiceSandbox] JSON enviado a MercadoPago:");
            Console.WriteLine(json);

            var client = new PreferenceClient();
            var preference = await client.CreateAsync(request);

            return new CheckoutResponseDto
            {
                Id = preference.Id,
                InitPoint = preference.InitPoint,
                SandboxInitPoint = preference.SandboxInitPoint
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PaymentServiceSandbox] Error Mercado Pago: {ex}");
            throw new Exception($"Error al crear la preferencia de pago: {ex.Message}", ex);
        }
    }

    public Task<PaymentResponseDto> CreatePaymentAsync(PaymentRequestDto dto)
    {
        throw new NotImplementedException("Método de pago directo no implementado en Sandbox.");
    }
}

