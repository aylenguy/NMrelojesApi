using Application.Model.Request;
using Application.Model.Response;
using Domain.Entities;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> CreatePaymentAsync(PaymentRequestDto dto);
        Task<CheckoutResponseDto> CreateCheckoutPreferenceAsync(CheckoutRequestDto dto);

       // Task<Venta?> GetByExternalReferenceAsync(string externalReference);
        //void UpdateStatus(int ventaId, VentaStatus newStatus);
      

    }
}
