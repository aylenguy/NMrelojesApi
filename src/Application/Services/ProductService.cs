using Application.Interfaces;
using Application.Models.DTOs;
using Domain.Entities;
using Domain.Interfaces;


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
                Images = p.Images ?? new List<string>(), // ✅ ahora lista
                Description = p.Description,
                Color = p.Color,
                Specs = string.IsNullOrEmpty(p.Specs)
                    ? new List<string>()
                    : p.Specs.Split(',').ToList(),
                Stock = p.Stock,
                Brand = p.Brand
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
                Images = new List<string> { imageFileName }, // ✅ ahora es lista
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



