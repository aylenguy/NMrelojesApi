namespace Application.Models
{
    public class VentaDto
    {
        // Cliente (null o <=0 si es invitado)
        public int? ClientId { get; set; }

        // 👇 Email obligatorio para invitados
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerLastname { get; set; } = string.Empty;

        public string ExternalReference { get; set; } = string.Empty;

        public string CouponCode { get; set; } = string.Empty;

        // Dirección mínima desde frontend
        public string ShippingAddress { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;

        // Envío y pago
        public string PaymentMethod { get; set; } = string.Empty;
        public string ShippingMethod { get; set; } = string.Empty;
        public decimal ShippingCost { get; set; }

        public string Province { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;

        // Notas del cliente
        public string Notes { get; set; } = string.Empty;

        // ✅ Items de la venta (ahora habilitado)
        public List<VentaItemDto> Items { get; set; } = new();
    }

    public class VentaItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }    // Precio unitario del producto
        public decimal Subtotal { get; set; }     // Subtotal = UnitPrice * Quantity
        public string ProductName { get; set; } = string.Empty;
    }
}
