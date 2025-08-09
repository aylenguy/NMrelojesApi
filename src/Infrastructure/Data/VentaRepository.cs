using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Data
{
    public class VentaRepository : IVentaRepository
    {
        private readonly ApplicationContext _context;

        public VentaRepository(ApplicationContext context)
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

        public Venta? GetById(int id)
        {
            return _context.Ventas
                .Include(v => v.Client)
                .Include(v => v.DetalleVentas)
                    .ThenInclude(dv => dv.Product)
                .SingleOrDefault(v => v.Id == id);
        }

        public void Add(Venta venta)
        {
            _context.Ventas.Add(venta);
            _context.SaveChanges(); // Esto ya asigna venta.Id
        }
        public void Update(Venta venta)
        {
            _context.Ventas.Update(venta);
        }

        public void Delete(Venta venta)
        {
            _context.Ventas.Remove(venta);
        }
    }
}
