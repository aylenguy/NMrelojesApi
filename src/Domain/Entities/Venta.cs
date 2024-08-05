using Domain.Entities.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Venta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [ForeignKey("Client")]
        public int ClientId { get; set; }
        public required Client Client { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public required Product Product { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
