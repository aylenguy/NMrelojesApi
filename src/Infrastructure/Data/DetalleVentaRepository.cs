using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class DetalleVentaRepository : RepositoryBase<DetalleVenta>, IDetalleVentaRepository
    {
        private readonly ApplicationContext _context;
        public DetalleVentaRepository(ApplicationContext context) : base(context)
        {
            _context = context;
        }

        public DetalleVenta? GetById(int id)
        {
            return _context.DetalleVentas
                .Include(dv => dv.Product)
                .Include(dv => dv.Venta)
                .ThenInclude(v => v.Client)
                .SingleOrDefault(x => x.Id == id);
        }

        public List<DetalleVenta> GetAllByVenta(int ventaId)
        {
            return _context.DetalleVentas
                .Include(dv => dv.Product)
                .Include(dv => dv.Venta)
                .ThenInclude(v => v.Client)
                .Where(dv => dv.VentaId == ventaId)
                .ToList();
        }

        public List<DetalleVenta> GetAllByProduct(int productId)
        {
            return _context.DetalleVentas
                .Include(dv => dv.Product)
                .Include(dv => dv.Venta)
                .ThenInclude(v => v.Client)
                .Where(dv => dv.ProductId == productId)
                .ToList();
        }

        public List<DetalleVenta> GetAllByClient(int clientId)
        {
            return _context.DetalleVentas
                .Include(dv => dv.Product)
                .Include(dv => dv.Venta)
                .ThenInclude(v => v.Client)
                .Where(dv => dv.Venta.ClientId == clientId)
                .ToList();
        }

        public bool VentaExists(int ventaId)
        {
            return _context.Ventas.Any(v => v.Id == ventaId);
        }

        public Product? GetProduct(int productId)
        {
            return _context.Products.Find(productId);
        }
    }
}
