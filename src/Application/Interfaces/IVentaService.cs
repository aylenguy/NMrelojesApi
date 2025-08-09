using Application.Models;
using Domain.Entities;
using System.Collections.Generic;

namespace Application.Interfaces
{
    public interface IVentaService
    {
        List<Venta> GetAllByClient(int clientId);
        Venta? GetById(int id);
        int AddVenta(VentaDto dto);
        void DeleteVenta(int id);
        void UpdateVenta(int id, VentaDto dto);
    }
}
