using Domain.Entities;

namespace Application.Model
{
    public class VentaDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int ClientId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public static VentaDto Create(Venta venta)
        {
            return new VentaDto
            {
                Id = venta.Id,
                Date = venta.Date,
                ClientId = venta.ClientId,
                ProductId = venta.ProductId,
                Quantity = venta.Quantity
            };
        }

        public static List<VentaDto> CreateList(List<Venta> ventas)
        {
            return ventas.Select(v => Create(v)).ToList();
        }
    }
}
