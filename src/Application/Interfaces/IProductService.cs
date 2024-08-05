using Application.Model.Request;
using ConsultaAlumnos.Application.Models;
using Domain.Entities;
using System.Collections.Generic;

namespace Application.Interfaces
{
    public interface IProductService
    {
        ProductDto Create(ProductCreateRequest productCreateRequest);
        void Delete(int id);
        List<ProductDto> GetAll();
        ProductDto GetById(int id);
        void Update(int id, ProductUpdateRequest productUpdateRequest);
    }
}
