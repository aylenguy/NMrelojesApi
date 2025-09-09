using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Data
{
    public class VentaRepository : RepositoryBase<Venta>, IVentaRepository
    {
        private readonly ApplicationContext _context;

        public VentaRepository(ApplicationContext context) : base(context)
        {
            _context = context;
        }

        public List<Venta> GetAllByClient(int clientId)
        {
            return _context.Ventas
                .Include(v => v.Client)
                .Include(v => v.DetalleVentas)
                    .ThenInclude(dv => dv.Product)
                .Where(v => v.ClientId == clientId)
                .ToList();
        }

        // 🔹 Nuevo método para Admin: traer todas las ventas
        public List<Venta> GetAll()
        {
            return _context.Ventas
                .Include(v => v.Client)
                .Include(v => v.DetalleVentas)
                    .ThenInclude(dv => dv.Product)
                .ToList();
        }

        public async Task<Venta?> GetByExternalReferenceAsync(string externalReference)
        {
            return await _context.Ventas
                .Include(v => v.Client)
                .Include(v => v.DetalleVentas)
                    .ThenInclude(dv => dv.Product)
                .FirstOrDefaultAsync(v => v.ExternalReference == externalReference);
        }

        public async Task UpdateAsync(Venta venta)
        {
            _context.Ventas.Update(venta);
            await _context.SaveChangesAsync();
        }

        public Venta? GetById(int id)
        {
            return _context.Ventas
                .Include(v => v.Client)
                .Include(v => v.DetalleVentas)
                    .ThenInclude(dv => dv.Product)
                .FirstOrDefault(v => v.Id == id);
        }

        public async Task<Venta> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Ventas
                .Include(v => v.DetalleVentas)
                    .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

    }
}
