using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IDetalleVentaRepository : IRepositoryBase<DetalleVenta>
    {
        DetalleVenta? GetById(int id);
        List<DetalleVenta> GetAllByVenta(int orderId);
        List<DetalleVenta> GetAllByProduct(int productId);
        List<DetalleVenta> GetAllByClient(int clientId);
        bool VentaExists(int saleOrderId);
        Product? GetProduct(int productId);

        void Delete(int id);

        int Add(DetalleVenta detalle);
    }
}
