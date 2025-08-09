using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Venta
    {
        public int Id { get; set; }
        public int ClientId { get; set; }

        // Inicializada para evitar advertencia de null
        public Client Client { get; set; } = null!;

        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }

        // Inicializada para evitar advertencia de null
        public ICollection<DetalleVenta> DetalleVentas { get; set; } = new List<DetalleVenta>();
    }
}
