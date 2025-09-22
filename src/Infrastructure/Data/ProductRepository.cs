using Infrastructure.Data;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;


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

        public Product? GetById(int id)
        {
            return _ctx.Products.SingleOrDefault(p => p.Id == id);
        }

        public Product? Get(int id)
        {
            return _ctx.Products.FirstOrDefault(p => p.Id == id);
        }

        public Product? Get(string name)
        {
            return _ctx.Products.FirstOrDefault(p => p.Name == name);
        }

        public List<Product> GetAll()
        {
            return _ctx.Products.ToList();
        }

        public void Update(Product p)
        {
            _ctx.Products.Update(p);
            _ctx.SaveChanges();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _ctx.Products.SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task UpdateAsync(Product p)
        {
            _ctx.Products.Update(p);
            await _ctx.SaveChangesAsync();
        }


    }
}


