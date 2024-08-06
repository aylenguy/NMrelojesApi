using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class DetalleVenta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Amount { get; set; }
        public decimal UnitPrice { get; set; }
        [ForeignKey("ProductId")]
        public int ProductId { get; set; } // Clave foránea para la relación con Producto
        public Product Product { get; set; }
        [ForeignKey("VentaId")]
        public int VentaId { get; set; } // Clave foránea para la relación con OrdenDeVenta
        public Venta Venta { get; set; }
        public decimal Total { get { return Product.Price * Amount; } }
    }
}
