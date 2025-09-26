namespace Application.Models.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }

        // Nombre del producto
        public string Name { get; set; } = string.Empty;

        // Precio actual
        public decimal Price { get; set; }

        // Precio anterior (opcional)
        public decimal? OldPrice { get; set; }

        // Imagen principal (URL o base64)
        public List<string> Images { get; set; } = new List<string>();


        // Descripción del producto
        public string Description { get; set; } = string.Empty;

        // Color (opcional)
        public string? Color { get; set; }

        // Lista de características o especificaciones
        public List<string> Specs { get; set; } = new();

        // Cantidad en stock
        public int Stock { get; set; }

        public string? Brand { get; set; }
    }
}
