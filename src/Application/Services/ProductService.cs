using Application.Interfaces;
using Application.Models.DTOs;
using Application.Models.Requests;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }
        public List<ProductDto> GetAllProducts()
        {
            return _repository.Get().Select(p => new ProductDto
            {
                Id = p.Id,
                Nombre = p.Name,
                Precio = p.Price,
                PrecioAnterior = p.OldPrice,
                Imagen = p.Image,
                Descripcion = p.Description,
                Color = p.Color,
                Caracteristicas = string.IsNullOrEmpty(p.Specs)
    ? new List<string>()
    : p.Specs.Split(',').ToList()

            }).ToList();
        }

        public Product? Get(string name)
        {
            return _repository.Get(name);
        }

        public Product? GetById(int id)
        {
            return _repository.Get(id);
        }

        public Product? Get(int id)
        {
            return _repository.Get(id);
        }

        public int AddProduct(ProductCreateRequest request)
        {
            var product = new Product()
            {
                Name = request.Name,
                Price = request.Price,
                OldPrice = request.OldPrice,
                Image = request.Image,
                Description = request.Description,
                Color = request.Color,
                Specs = string.Join(",", request.Caracteristicas),
                Stock = request.Stock
            };
            return _repository.Add(product).Id;
        }


        public void DeleteProduct(int id)
        {
            var productToDelete = _repository.Get(id);
            if (productToDelete != null)
            {
                _repository.Delete(productToDelete);
            }
        }


        public void UpdateProduct(int id, ProductUpdateRequest request)
        {
            var productToUpdate = _repository.Get(id);
            if (productToUpdate != null)
            {
                productToUpdate.Price = request.Price;
                productToUpdate.Stock = request.Stock;
                productToUpdate.OldPrice = request.OldPrice;
                productToUpdate.Image = request.Image;
                productToUpdate.Description = request.Description;
                productToUpdate.Color = request.Color;
                productToUpdate.Specs = string.Join(",", request.Caracteristicas);

                _repository.Update(productToUpdate);
            }
        }
    }
}



