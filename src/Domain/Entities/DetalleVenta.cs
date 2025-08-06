using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public int ProductId { get; set; }
        public required Product Product { get; set; }

        [ForeignKey("VentaId")]
        public int VentaId { get; set; }
        public required Venta Venta { get; set; }

        public decimal Total => UnitPrice * Amount;
    }
}
