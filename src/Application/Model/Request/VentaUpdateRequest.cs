namespace Application.Model.Request
{
    public class VentaUpdateRequest
    {
        public DateTime Date { get; set; }
        public int ClientId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
