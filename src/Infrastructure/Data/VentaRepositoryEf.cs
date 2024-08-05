using Domain.Entities;
using Domain.Interfaces;
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

        public Venta GetVentaById(int id)
        {
            return _context.Ventas.Find(id);
        }

        public IEnumerable<Venta> GetAllVentas()
        {
            return _context.Ventas.ToList();
        }

        public Venta AddVenta(Venta venta)
        {
            _context.Ventas.Add(venta);
            _context.SaveChanges();
            return venta;
        }

        public void UpdateVenta(Venta venta)
        {
            _context.Ventas.Update(venta);
            _context.SaveChanges();
        }

        public void DeleteVenta(int id)
        {
            var venta = _context.Ventas.Find(id);
            if (venta != null)
            {
                _context.Ventas.Remove(venta);
                _context.SaveChanges();
            }
        }
    }
}