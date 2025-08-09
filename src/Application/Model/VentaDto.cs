using System.Collections.Generic;

namespace Application.Models
{
    public class VentaDto
    {
        public int ClientId { get; set; }
        public List<DetalleVentaDto> Items { get; set; } = new List<DetalleVentaDto>();
    }

    public class DetalleVentaDto
    {
        public int ProductId { get; set; }
        public int VentaId { get; set; } // opcional para AddDetalleVenta
        public int Cantidad { get; set; } // usamos Cantidad, no Amount
    }
}