using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Data
{
    public class DetalleVentaRepository : RepositoryBase<DetalleVenta>, IDetalleVentaRepository
    {
        private readonly ApplicationContext _context;

        public DetalleVentaRepository(ApplicationContext context) : base(context)
        {
            _context = context;
        }

        public List<DetalleVenta> GetAllByVenta(int ventaId)
        {
            return _context.DetalleVentas
                .Include(dv => dv.Product)
                .Where(dv => dv.VentaId == ventaId)
                .ToList();
        }

        public List<DetalleVenta> GetAllByClient(int clientId)
        {
            return _context.DetalleVentas
                .Include(dv => dv.Venta)
                .ThenInclude(v => v.Client) // si querés incluir datos del cliente
                .Where(dv => dv.Venta.ClientId == clientId)
                .ToList();
        }

        public List<DetalleVenta> GetAllByProduct(int productId)
        {
            return _context.DetalleVentas
                .Include(dv => dv.Venta)
                .Where(dv => dv.ProductId == productId)
                .ToList();
        }

        public DetalleVenta? GetById(int id)
        {
            return _context.DetalleVentas
                .Include(dv => dv.Product)
                .Include(dv => dv.Venta)
                .FirstOrDefault(dv => dv.Id == id);
        }

        public DetalleVenta Add(DetalleVenta detalle)
        {
            _context.DetalleVentas.Add(detalle);
            _context.SaveChanges();
            return detalle;
        }

        // Si tu RepositoryBase ya tiene Update y Delete, no hace falta reescribirlos
    }
}
