using Domain.Entities;
using System.Collections.Generic;

namespace Application.Interfaces
{
    public interface IVentaService
    {
        Venta GetVentaById(int id);
        IEnumerable<Venta> GetAllVentas();
        void AddVenta(Venta venta);
        void UpdateVenta(Venta venta);
        void DeleteVenta(int id);
    }
}
