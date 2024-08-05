using Application.Interfaces;
using Application.Model;
using Application.Model.Request;
using ConsultaAlumnos.Application.Models;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using System.Collections.Generic;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public ProductDto GetById(int id)
        {
            var product = _productRepository.GetById(id)
                          ?? throw new NotFoundException(nameof(Product), id);
            return ProductDto.Create(product);
        }

        public List<ProductDto> GetAll()
        {
            var list = _productRepository.GetAll();
            return ProductDto.CreateList(list);
        }

        public Product Create(ProductCreateRequest productCreateRequest)
        {
            var product = new Product
            {
                Name = productCreateRequest.Name,
                Price = productCreateRequest.Price,
                Color = productCreateRequest.Color,
                Stock = productCreateRequest.Stock
            };

            _productRepository.Add(product);
           
            return product;
        }

        public void Update(int id, ProductUpdateRequest productUpdateRequest)
        {
            var product = _productRepository.GetById(id)
                          ?? throw new NotFoundException(nameof(Product), id);

            if (productUpdateRequest.Price > 0) product.Price = productUpdateRequest.Price;
            if (productUpdateRequest.Stock >= 0) product.Stock = productUpdateRequest.Stock;

            _productRepository.Update(product);
           
        }

        public void Delete(int id)
        {
            var product = _productRepository.GetById(id)
                          ?? throw new NotFoundException(nameof(Product), id);
            _productRepository.Delete(product);
           
        }

        ProductDto IProductService.Create(ProductCreateRequest productCreateRequest)
        {
            throw new NotImplementedException();
        }
    }
}
