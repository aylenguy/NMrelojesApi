using Domain.Entities;
using Domain.Interface;
using System.Collections.Generic;

namespace Domain.Interfaces
{
    public interface IProductRepository : IRepositoryBase<Product>
    {
        Product? GetById(int id);
        List<Product> GetByName(string name);
        new List<Product> GetAll();
    }
}
