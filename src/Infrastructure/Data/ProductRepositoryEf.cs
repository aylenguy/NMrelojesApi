using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class ProductRepositoryEf : IProductRepository
    {
        private readonly ApplicationContext _context;

        public ProductRepositoryEf(ApplicationContext context)
        {
            _context = context;
        }

        public Product Add(Product product)
        {
            _context.Product.Add(product);
            _context.SaveChanges();
            return product;
        }

        public void Delete(Product product)
        {
            _context.Remove(product);
            _context.SaveChanges();

        }

        public List<Product> GetAll()
        {
            return _context.Product.ToList();
        }

        public Product? GetById(int id)
        {
            return _context.Product
                .FirstOrDefault(x => x.Id == id);
        }

        public void Update(Product product)
        {
            _context.Update(product);
            _context.SaveChanges();

        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
