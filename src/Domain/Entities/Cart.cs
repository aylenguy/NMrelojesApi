using Domain.Entities;

public class Cart
{
    public int Id { get; set; }
    public int? ClientId { get; set; }
    public Client? Client { get; set; }

    public string? GuestId { get; set; } // 👈 para invitados

    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
