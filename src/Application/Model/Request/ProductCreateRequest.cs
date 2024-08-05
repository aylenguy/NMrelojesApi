namespace Application.Model.Request
{
    public class ProductCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Color { get; set; } = string.Empty;
        public int Stock { get; set; }
    }
}
