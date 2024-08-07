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
                .Include(so => so.Client)
                .Include(so => so.DetalleVentas)
                .ThenInclude(so => so.Product)
                .Where(r => r.ClientId == clientId)
                .ToList();
        }

        public Venta? GetById(int id)
        {
            return _context.Ventas
                .Include(r => r.Client)
                .Include(r => r.DetalleVentas)
                .ThenInclude(so => so.Product)
                .SingleOrDefault(x => x.Id == id);
        }
    }
}
