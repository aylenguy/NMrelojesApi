using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Model.Request
{
    public class ProductCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public int Price { get; set; } = 0;
        public string Size { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
    }
}
