using Application.Models;
using Application.Models.Requests;
using Domain.Entities;
using System.Collections.Generic;

namespace Application.Interfaces
{
    public interface IDetalleVentaService
    {
        List<DetalleVenta> GetAllByVenta(int ventaId);
        DetalleVenta? GetById(int id);

        int AddDetalleVenta(DetalleVentaDto dto);
        void UpdateDetalleVenta(int id, DetalleVentaUpdateRequest request);
        void DeleteDetalleVenta(int id);
    }
}
