using Application.Models;

public class VentaResponseDto
{
    public int OrderId { get; set; }
    public int? ClientId { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerLastname { get; set; } = string.Empty;

    // Dirección
    public string Street { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;

    // Pedido
    public string ShippingMethod { get; set; } = string.Empty;
    public decimal ShippingCost { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;

    // Venta
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Total { get; set; }

    public string ExternalReference { get; set; } = string.Empty;

    // Mercado Pago
    public string InitPoint { get; set; } = string.Empty;
    public string SandboxInitPoint { get; set; } = string.Empty;

    // Items
    public List<VentaItemDto> Items { get; set; } = new();

}

public class VentaItemResponseDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }

    public int CurrentStock { get; set; }
}
