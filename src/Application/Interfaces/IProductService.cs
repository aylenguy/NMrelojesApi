using Domain.Entities;
using Application.Models.DTOs;
using Application.Models.Requests;

namespace Application.Interfaces
{
    public interface IProductService
    {
        List<ProductDto> GetAllProducts();
        Product? Get(string name);
        Product? Get(int id); // entidad cruda (si la necesitás internamente)

        ProductDto? GetByIdDto(int id); // DTO para el controller

        int AddProduct(ProductCreateRequest request, List<string> imageFileNames);

        void DeleteProduct(int id);
        void UpdateProduct(int id, ProductUpdateRequest request);
    }
}
