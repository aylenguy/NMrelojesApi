using MercadoPago.Config;
using MercadoPago.Client.Preference;
using Application.Model.Request;
using Microsoft.Extensions.Configuration;
using Application.Interfaces;
using Application.Model.Response;
using System;
using System.Linq;
using System.Threading.Tasks;

public class PaymentServiceSandbox : IPaymentService
{
    private readonly string _accessToken;

    public PaymentServiceSandbox(IConfiguration configuration)
    {
        _accessToken = configuration["MercadoPago:AccessToken"];
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
                    Success = dto.BackUrls?.Success ?? "https://localhost:5173/checkout/success",
                    Failure = dto.BackUrls?.Failure ?? "https://localhost:5173/checkout/failure",
                    Pending = dto.BackUrls?.Pending ?? "https://localhost:5173/checkout/pending"
                },

                AutoReturn = null, // No confiar en AutoReturn para actualizar stock/venta

                ExternalReference = dto.ExternalReference,
                NotificationUrl = dto.NotificationUrl ?? "https://427eeb99434f.ngrok-free.app/api/Payment/webhook"
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
