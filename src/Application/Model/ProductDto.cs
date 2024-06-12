using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsultaAlumnos.Application.Models;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } 
    public int Price { get; set; } 
    public string Size { get; set; } 
    public string Color { get; set; } 

    public static ProductDto Create(Product product)
    {
        var dto = new ProductDto();
        dto.Id = product.Id;
        dto.Name = product.Name;
        dto.Description = product.Description;
        dto.Price = product.Price;
        dto.Size = product.Size;
        dto.Color = product.Color;


        return dto;
    }

    public static List<ProductDto> CreateList(IEnumerable<Product> product)
    {
        List<ProductDto> listDto = [];
        foreach (var p in product)
        {
            listDto.Add(Create(p));
        }

        return listDto;
    }

}