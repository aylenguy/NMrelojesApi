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
            return _repository.GetAll().Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                OldPrice = p.OldPrice,
                Image = p.Image,
                Description = p.Description,
                Color = p.Color,
                Specs = string.IsNullOrEmpty(p.Specs)
                    ? new List<string>()
                    : p.Specs.Split(',').ToList(),
                Stock = p.Stock, // 🔹 Ahora sí lo devuelve
                    Brand = p.Brand  // 🔥 Nuevo: mapeo de la marca
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

        public int AddProduct(ProductCreateRequest request, string imageFileName)
        {
            var product = new Product()
            {
                Name = request.Name,
                Price = request.Price,
                OldPrice = request.OldPrice,
                Image = imageFileName, // ahora sí existe
                Description = request.Description,
                Color = request.Color,
                Specs = string.Join(",", request.Caracteristicas),
                Stock = request.Stock,
                Brand = request.Brand
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
                // 🔹 Nombre
                if (!string.IsNullOrWhiteSpace(request.Name))
                    productToUpdate.Name = request.Name;

                // Campos numéricos
                productToUpdate.Price = request.Price;
                productToUpdate.Stock = request.Stock;
                productToUpdate.OldPrice = request.OldPrice;

                // Texto
                if (!string.IsNullOrWhiteSpace(request.Description))
                    productToUpdate.Description = request.Description;

                if (!string.IsNullOrWhiteSpace(request.Color))
                    productToUpdate.Color = request.Color;

                if (!string.IsNullOrWhiteSpace(request.Brand))
                    productToUpdate.Brand = request.Brand;


                // Características
                if (request.Caracteristicas != null && request.Caracteristicas.Any())
                    productToUpdate.Specs = string.Join(",", request.Caracteristicas);

                _repository.Update(productToUpdate);
            }
        }


    }
}



