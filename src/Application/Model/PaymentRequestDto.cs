namespace Application.Model.Request
{
    public class PaymentRequestDto
    {
        public decimal Amount { get; set; }
        public string Token { get; set; } // token de la tarjeta (si aplica)
        public string Description { get; set; }
        public string PaymentMethodId { get; set; } // ej: "visa"
        public string PayerEmail { get; set; }
    }
}
