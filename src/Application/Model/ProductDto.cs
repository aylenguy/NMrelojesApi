namespace Application.Models.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }             // name del producto
        public decimal Precio { get; set; }            // precio actual
        public decimal? PrecioAnterior { get; set; }   // precio anterior, opcional
        public string Imagen { get; set; }             // URL o base64
        public string Descripcion { get; set; }        // descripción del producto
        public string Color { get; set; }              // color (opcional)
        public List<string> Caracteristicas { get; set; } = new(); // lista de características
    }
}
