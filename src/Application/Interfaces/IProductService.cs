
using Domain.Entities;
using Application.Models.DTOs;
using Application.Models.Requests;

namespace Application.Interfaces
{
    public interface IProductService
    {
        List<ProductDto> GetAllProducts(); // ← CAMBIO AQUÍ
        Product? Get(string name);
        Product? Get(int id);

        Product? GetById(int id);

        public int AddProduct(ProductCreateRequest request, List<string> imageFileNames);

        void DeleteProduct(int id);
        void UpdateProduct(int id, ProductUpdateRequest request);
    }
}