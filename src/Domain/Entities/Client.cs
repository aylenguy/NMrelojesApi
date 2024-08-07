using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Client : User
    {
        public string PhoneNumber { get; set; }
        public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
    }
}