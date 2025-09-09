namespace Domain.Entities
{

    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        // ✅ Renombrado
        public int Quantity { get; set; }

        // ✅ Guardamos el precio al momento de agregar al carrito
        public decimal UnitPrice { get; set; }

        // ✅ Subtotal calculado (no persistido)
        public decimal Subtotal => Quantity * (Product?.Price ?? 0);

    }
}
