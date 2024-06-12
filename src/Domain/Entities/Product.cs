using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public string PhotoUrl { get; set; }

        public Product(string name, int price, string size, string color, string description, string photoUrl)
        {
            Name = name;
            Price = price;
            Size = size;
            Color = color;
            Description = description;
            PhotoUrl = photoUrl;
        }

        
        
    }
}
