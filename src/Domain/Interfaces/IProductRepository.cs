using Domain.Entities;
using System.Collections.Generic;

namespace Domain.Interfaces
{
    public interface IProductRepository
    {
        Product? Get(int id);
        Product? Get(string name);
        List<Product> Get();
        Product Add(Product p);
        void Update(Product p);
        void Delete(Product p);
    }
}
