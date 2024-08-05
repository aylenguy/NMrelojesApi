using Domain.Entities;
using System.Collections.Generic;

namespace ConsultaAlumnos.Application.Models
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Color { get; set; } // Permitir valores NULL
        public int Stock { get; set; }

        public static ProductDto Create(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Color = product.Color,
                Stock = product.Stock
            };
        }

        public static List<ProductDto> CreateList(IEnumerable<Product> products)
        {
            var listDto = new List<ProductDto>();
            foreach (var product in products)
            {
                listDto.Add(Create(product));
            }
            return listDto;
        }
    }
}
