using Application.Models.Requests;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IProductService
    {
        List<Product> GetAllProducts();
        Product? Get(string name);
        Product? Get(int id);
        int AddProduct(ProductCreateRequest request);
        void DeleteProduct(int id);
        void UpdateProduct(int id, ProductUpdateRequest request);
    }
}
