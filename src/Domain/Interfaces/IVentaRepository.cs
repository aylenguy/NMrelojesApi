using Domain.Entities;
using System.Collections.Generic;

namespace Domain.Interfaces
{
    public interface IVentaRepository
    {
        Venta GetVentaById(int id);
        IEnumerable<Venta> GetAllVentas();
        Venta AddVenta(Venta venta);
        void UpdateVenta(Venta venta);
        void DeleteVenta(int id);
    }
}
