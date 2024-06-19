using Application.Interfaces;
using Application.Model.Request;
using ConsultaAlumnos.Application.Models;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ProductServices : IProductServices
    {
        private readonly IProductRepository _productRepository;

        public ProductServices(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
         public List<ProductDto> GetAll()
        {
            var list = _productRepository.GetAll();
            return ProductDto.CreateList(list);
        }

        public List<Product> GetAllFullData()
        {
            return _productRepository.GetAll();
        }

        public ProductDto GetById(int id) 
        {
            var obj = _productRepository.GetById(id)
                ?? throw new NotFoundException(nameof(Product), id);

            var dto = ProductDto.Create(obj);
            return dto;
            
        }
        public Product Create (ProductCreateRequest productCreateRequest)
        {
            var obj = new Product();
            obj.Name = productCreateRequest.Name;
            obj.PhotoUrl = productCreateRequest.PhotoUrl;
            obj.Description = productCreateRequest.Description;
            obj.Price = productCreateRequest.Price;
            obj.Size = productCreateRequest.Size;
            obj.Color = productCreateRequest.Color;

            return _productRepository.Add(obj);
        }
        public void Update(int id, ProductUpdateRequest productUpdateRequest)
        {

            var obj = _productRepository.GetById(id);

            if (obj == null)
                throw new NotFoundException(nameof(Product), id);

            if (productUpdateRequest.Price != 0) obj.Price = productUpdateRequest.Price;

            _productRepository.Update(obj);

        }

        public void Delete(int id)
        {
            var obj = _productRepository.GetById(id);

            if (obj == null)
                throw new NotFoundException(nameof(Product), id);

            _productRepository.Delete(obj);
        }

    }
}
