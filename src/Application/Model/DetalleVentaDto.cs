namespace Application.Models
{
    public class DetalleVentaDto
    {
        public int VentaId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }

        // 🔹 Campos necesarios para el frontend
        public decimal UnitPrice { get; set; }       // Precio unitario
        public decimal Subtotal { get; set; }        // UnitPrice * Cantidad
        public string ProductName { get; set; } = string.Empty;
    }

}
