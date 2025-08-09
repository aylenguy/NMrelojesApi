using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationContext _ctx;

        public ProductRepository(ApplicationContext ctx)
        {
            _ctx = ctx;
        }

        public Product Add(Product p)
        {
            _ctx.Products.Add(p);
            _ctx.SaveChanges();
            return p;
        }

        public void Delete(Product p)
        {
            _ctx.Products.Remove(p);
            _ctx.SaveChanges();
        }

        public Product? Get(int id)
        {
            return _ctx.Products.FirstOrDefault(p => p.Id == id);
        }

        public Product? Get(string name)
        {
            return _ctx.Products.FirstOrDefault(p => p.Name == name);
        }

        public List<Product> Get()
        {
            return _ctx.Products.ToList();
        }

        public void Update(Product p)
        {
            _ctx.Products.Update(p);
            _ctx.SaveChanges();
        }
    }
}

