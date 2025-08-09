using Application.Models;
using Application.Models.Requests;
using Domain.Entities;
using System.Collections.Generic;

namespace Application.Interfaces
{
    public interface IDetalleVentaService
    {
        List<DetalleVenta> GetAllByClient(int clientId);
        List<DetalleVenta> GetAllByProduct(int productId);
        List<DetalleVenta> GetAllByVenta(int orderId);
        DetalleVenta? GetById(int id);
        int AddDetalleVenta(DetalleVentaDto dto);
        void DeleteDetalleVenta(int id);
        void UpdateDetalleVenta(int id, DetalleVentaUpdateRequest request);
    }
}
