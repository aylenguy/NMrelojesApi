using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IProductRepository
    {
        Product Add(Product product);
        void Update(Product product);
        void Delete(Product product);
        List<Product> GetAll();
        Product? GetById(int id);
        void SaveChages();

    }
}


