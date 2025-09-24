using MercadoPago.Config;
using MercadoPago.Client.Preference;
using Application.Model.Request;
using Microsoft.Extensions.Configuration;
using Application.Interfaces;
using Application.Model.Response;


public class PaymentServiceSandbox : IPaymentService
{
    private readonly string _accessToken;
    private readonly IConfiguration _configuration; // 👈 faltaba esto

    public PaymentServiceSandbox(IConfiguration configuration)
    {
        _configuration = configuration; // 👈 guardamos la referencia
        _accessToken = _configuration["MercadoPago:AccessToken"];
        MercadoPagoConfig.AccessToken = _accessToken;
    }

    public async Task<CheckoutResponseDto> CreateCheckoutPreferenceAsync(CheckoutRequestDto dto)
    {
        try
        {
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
                    Success = dto.BackUrls?.Success ?? _configuration["FrontEndUrls:Success"],
                    Failure = dto.BackUrls?.Failure ?? _configuration["FrontEndUrls:Failure"],
                    Pending = dto.BackUrls?.Pending ?? _configuration["FrontEndUrls:Pending"]
                },

                AutoReturn = "approved", // 👈 para que vuelva solo al success si se aprueba

                ExternalReference = dto.ExternalReference,
                NotificationUrl = dto.NotificationUrl
                    ?? _configuration["BackEndUrls:NotificationUrl"] // 🔹 mejor desde config también
            };

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
