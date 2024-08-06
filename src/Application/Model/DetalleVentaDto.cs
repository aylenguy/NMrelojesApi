using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class DetalleVentaDto
    {
        public int ProductId { get; set; }
        public int VentaId { get; set; }
        public int Amount { get; set; }
    }
}
