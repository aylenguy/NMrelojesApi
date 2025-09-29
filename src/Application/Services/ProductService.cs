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

        // 🔹 Obtener TODOS los productos en formato DTO
        public List<ProductDto> GetAllProducts()
        {
            return _repository.GetAll().Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                OldPrice = p.OldPrice,
                Images = p.Images ?? new List<string>(), // ✅ siempre lista
                Description = p.Description,
                Color = p.Color,
                Specs = string.IsNullOrEmpty(p.Specs)
                    ? new List<string>()
                    : p.Specs.Split(',').ToList(),
                Stock = p.Stock,
                Brand = p.Brand
            }).ToList();
        }

        // 🔹 Obtener producto por nombre (entidad cruda)
        public Product? Get(string name)
        {
            return _repository.Get(name);
        }

        // 🔹 Obtener producto por ID (entidad cruda)
        public Product? Get(int id)
        {
            return _repository.Get(id);
        }

        // 🔹 Obtener producto por ID pero ya en DTO (para el Controller)
        public ProductDto? GetByIdDto(int id)
        {
            var p = _repository.Get(id);
            if (p == null) return null;

            return new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                OldPrice = p.OldPrice,
                Images = p.Images ?? new List<string>(), // ✅ lista garantizada
                Description = p.Description,
                Color = p.Color,
                Specs = string.IsNullOrEmpty(p.Specs) ? new List<string>() : p.Specs.Split(',').ToList(),
                Stock = p.Stock,
                Brand = p.Brand
            };
        }

        // 🔹 Agregar producto con múltiples imágenes
        public int AddProduct(ProductCreateRequest request, List<string> imageFileNames)
        {
            var product = new Product()
            {
                Name = request.Name,
                Price = request.Price,
                OldPrice = request.OldPrice,
                Images = imageFileNames ?? new List<string>(), // ✅ guarda todas
                Description = request.Description,
                Color = request.Color,
                Specs = string.Join(",", request.Caracteristicas),
                Stock = request.Stock,
                Brand = request.Brand
            };

            return _repository.Add(product).Id;
        }

        // 🔹 Eliminar producto
        public void DeleteProduct(int id)
        {
            var productToDelete = _repository.Get(id);
            if (productToDelete != null)
            {
                _repository.Delete(productToDelete);
            }
        }

        // 🔹 Actualizar producto
        public void UpdateProduct(int id, ProductUpdateRequest request)
        {
            var productToUpdate = _repository.Get(id);
            if (productToUpdate != null)
            {
                // Campos de texto
                if (!string.IsNullOrWhiteSpace(request.Name))
                    productToUpdate.Name = request.Name;

                if (!string.IsNullOrWhiteSpace(request.Description))
                    productToUpdate.Description = request.Description;

                if (!string.IsNullOrWhiteSpace(request.Color))
                    productToUpdate.Color = request.Color;

                if (!string.IsNullOrWhiteSpace(request.Brand))
                    productToUpdate.Brand = request.Brand;

                // Campos numéricos
                productToUpdate.Price = request.Price;
                productToUpdate.Stock = request.Stock;
                productToUpdate.OldPrice = request.OldPrice;

                // Características
                if (request.Caracteristicas != null && request.Caracteristicas.Any())
                    productToUpdate.Specs = string.Join(",", request.Caracteristicas);

                _repository.Update(productToUpdate);
            }
        }
    }
}
