
public class CartItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }   // ✅ unificado
    public decimal UnitPrice { get; set; } // ✅ renombrado de Price

    public string Brand { get; set; }  // <-- nuevo
    public string ImageUrl { get; set; }

    public decimal Subtotal => Quantity * UnitPrice;
}