namespace Application.DTOs
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;

        // ✅ Cantidad pedida por el cliente
        public int Quantity { get; set; }

        // ✅ Precio unitario del producto
        public decimal UnitPrice { get; set; }

        // ✅ Marca del producto
        public string Brand { get; set; } = string.Empty;

        // ✅ Soporta varias imágenes del producto
        public List<string> Images { get; set; } = new List<string>();


        // ✅ Stock disponible
        public int Stock { get; set; }

        // ✅ Calculado automáticamente
        public decimal Subtotal => Quantity * UnitPrice;
    }
}
