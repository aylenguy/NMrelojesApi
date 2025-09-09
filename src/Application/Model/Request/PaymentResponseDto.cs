namespace Application.Model.Response
{
    public class PaymentResponseDto
    {
        public long? Id { get; set; }
        public string? Status { get; set; }
        public string? StatusDetail { get; set; }
        public string? InitPoint { get; set; }
    }
}
