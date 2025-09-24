namespace Domain.Entities
{
    public class Venta
    {
        public int Id { get; set; }

        // Cliente (puede ser null si es invitado)
        public int? ClientId { get; set; }
        public Client? Client { get; set; }

        // 👇 Nuevo: guardar email aunque sea invitado
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerLastname { get; set; } = string.Empty;
        public string ExternalReference { get; set; } = string.Empty;


        public string StatusDetail { get; set; } = string.Empty;
        public string PaymentId { get; set; } = string.Empty;
        public decimal TransactionAmount { get; set; }

        // Dirección
        public string Street { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;

        public string PaymentStatus { get; set; } = string.Empty;


        // Estado
        public VentaStatus Status { get; set; }



        // Envío y pago
        public string ShippingMethod { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public decimal ShippingCost { get; set; } = 0;
        public string PaymentMethod { get; set; } = string.Empty;
        public string DeliveryMethod { get; set; } = string.Empty;

        // Generales
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public decimal Total { get; set; }
        public string Notes { get; set; } = string.Empty;

        // Detalles
        public ICollection<DetalleVenta> DetalleVentas { get; set; } = new List<DetalleVenta>();
    }

    public enum VentaStatus
    {
        Pendiente,
        Pagado,
        Enviado,
        Entregado,
        Cancelado
    }
}
