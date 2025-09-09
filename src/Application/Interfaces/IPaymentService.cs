using Application.Model.Request;
using Application.Model.Response;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> CreatePaymentAsync(PaymentRequestDto dto);
        Task<CheckoutResponseDto> CreateCheckoutPreferenceAsync(CheckoutRequestDto dto);
    }
}
