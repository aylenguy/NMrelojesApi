using System.ComponentModel.DataAnnotations;

public class ProductCreateRequest
{
    [Required]
    public string Name { get; set; }

    [Required]
    public decimal Price { get; set; }

    public decimal? OldPrice { get; set; }

    

    public string Description { get; set; }

    public string Color { get; set; }

    public string? Brand { get; set; }

    public List<string> Caracteristicas { get; set; } = new();

    [Required]
    public int Stock { get; set; }
}
