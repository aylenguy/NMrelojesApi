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

        public int Add(DetalleVenta detalle)
        {
            _context.DetalleVentas.Add(detalle);
            _context.SaveChanges();
            return detalle.Id;
        }

        public void Delete(int id)
        {
            var detalle = _context.DetalleVentas.FirstOrDefault(d => d.Id == id);
            if (detalle != null)
            {
                _context.DetalleVentas.Remove(detalle);
                _context.SaveChanges();
            }
        }
        public DetalleVenta? GetById(int id)
        {
            return _context.DetalleVentas
                .Include(dv => dv.Product)
                .Include(dv => dv.Venta)
                .ThenInclude(v => v.Client)
                .SingleOrDefault(dv => dv.Id == id);
        }

        public List<DetalleVenta> GetAllByVenta(int ventaId)
        {
            return GetDetalles().Where(dv => dv.VentaId == ventaId).ToList();
        }

        public List<DetalleVenta> GetAllByProduct(int productId)
        {
            return GetDetalles().Where(dv => dv.ProductId == productId).ToList();
        }

        public List<DetalleVenta> GetAllByClient(int clientId)
        {
            return GetDetalles().Where(dv => dv.Venta.ClientId == clientId).ToList();
        }

        public Product? GetProduct(int productId)
        {
            return _context.Products.Find(productId);
        }

        public bool VentaExists(int ventaId)
        {
            return _context.Ventas.Any(v => v.Id == ventaId);
        }

        private IQueryable<DetalleVenta> GetDetalles()
        {
            return _context.DetalleVentas
                .Include(dv => dv.Product)
                .Include(dv => dv.Venta)
                .ThenInclude(v => v.Client);
        }
    }
}
