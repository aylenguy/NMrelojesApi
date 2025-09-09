namespace Domain.Entities
{
    public class DetalleVenta
    {
        public int Id { get; set; }
        public int VentaId { get; set; }
        public Venta Venta { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        // 👌 En ventas sí guardamos Subtotal (snapshot)
        public decimal Subtotal { get; set; }
    }
}
