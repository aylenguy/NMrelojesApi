namespace Application.DTOs
{
    public class CartDto
    {
        public int Id { get; set; }
       

        public int? ClientId { get; set; }     // 👈 ahora es nullable
        public string? GuestId { get; set; }
        public List<CartItemDto> Items { get; set; } = new();

        public decimal Total => Items?.Sum(i => i.Subtotal) ?? 0;
    }

}