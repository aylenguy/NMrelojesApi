using Domain.Entities;
using System.Collections.Generic;

namespace Domain.Interfaces
{
  
        public interface IDetalleVentaRepository : IRepositoryBase<DetalleVenta>
        {
            // Métodos específicos de DetalleVenta
            List<DetalleVenta> GetAllByVenta(int ventaId);
            List<DetalleVenta> GetAllByClient(int clientId);
            List<DetalleVenta> GetAllByProduct(int productId);
        }
    
}
