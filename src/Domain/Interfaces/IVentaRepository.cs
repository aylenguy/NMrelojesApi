using Domain.Entities;
using System.Collections.Generic;

namespace Application.Interfaces
{
    public interface IVentaRepository
    {
        List<Venta> GetAllByClient(int clientId);
        Venta? GetById(int id);
        void Add(Venta venta);
        void Update(Venta venta);
        void Delete(Venta venta);
    }
}
