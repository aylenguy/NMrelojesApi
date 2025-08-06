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
            return GetVentas()
                .Where(r => r.ClientId == clientId)
                .ToList();
        }

        public Venta? GetById(int id)
        {
            return GetVentas()
                .SingleOrDefault(x => x.Id == id);
        }

        private IQueryable<Venta> GetVentas()
        {
            return _context.Ventas
                .Include(r => r.Client)
                .Include(r => r.DetalleVentas)
                .ThenInclude(dv => dv.Product);
        }
    }
}
