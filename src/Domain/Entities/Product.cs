using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string? Color { get; set; }

        [Required]
        public int Stock { get; set; }

        [NotMapped]
        public StockStatus StockTshirt => Stock > 0 ? StockStatus.Available : StockStatus.OutOfStock;

        public enum StockStatus
        {
            OutOfStock,
            Available
        }

        public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
    }
}
