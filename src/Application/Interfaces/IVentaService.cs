using Application.Models;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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