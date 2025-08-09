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
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationContext _ctx;

        public OrderRepository(ApplicationContext ctx)
        {
            _ctx = ctx;
        }

        public Venta Add(Venta venta)
        {
            _ctx.Ventas.Add(venta);
            _ctx.SaveChanges();
            return venta;
        }

        public void AddItem(DetalleVenta detalle)
        {
            _ctx.DetalleVentas.Add(detalle);
            _ctx.SaveChanges();
        }

        public Venta GetById(int id)
        {
            return _ctx.Ventas
                .Include(v => v.DetalleVentas)
                .ThenInclude(d => d.Product)
                .FirstOrDefault(v => v.Id == id)!;
        }

        public List<Venta> GetByClientId(int clientId)
        {
            return _ctx.Ventas
                .Where(v => v.ClientId == clientId)
                .Include(v => v.DetalleVentas)
                .ThenInclude(d => d.Product)
                .ToList();
        }

        public void Update(Venta venta)
        {
            _ctx.Ventas.Update(venta);
            _ctx.SaveChanges();
        }
    }
}
