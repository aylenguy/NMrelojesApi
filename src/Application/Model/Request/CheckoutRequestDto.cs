public class CheckoutRequestDto
{
    public string Description { get; set; } = "Compra en la tienda";
    public string? PayerEmail { get; set; }
    public string CurrencyId { get; set; } = "ARS";
    public string? ExternalReference { get; set; }

    // Lista de productos
    public List<CheckoutItemDto> Items { get; set; } = new();

    // ✅ URLs de redirección
    public BackUrlsDto? BackUrls { get; set; }

    // ✅ Webhook opcional
    public string? NotificationUrl { get; set; }

    public string? AutoReturn { get; set; }
}

public class CheckoutItemDto
{
    public string Title { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class BackUrlsDto
{
    public string? Success { get; set; }
    public string? Failure { get; set; }
    public string? Pending { get; set; }
}
