using Domain.Entities;
using System.Collections.Generic;

namespace Domain.Interfaces
{
    public interface IOrderRepository
    {
        Venta Add(Venta venta);
        void Update(Venta venta);
        Venta GetById(int id);
        void AddItem(DetalleVenta detalle);
        List<Venta> GetByClientId(int clientId);
    }
}
