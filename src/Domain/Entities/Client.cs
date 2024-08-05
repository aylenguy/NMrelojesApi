using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{

    namespace Domain.Entities
    {
        public class Client : User
        {
            public ICollection<Venta>? Ventas { get; set; }
        }
    }
}
