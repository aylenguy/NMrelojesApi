using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks; // <- importante para Task

namespace Domain.Interfaces
{
    public interface IProductRepository
    {
        // Obtiene un producto por su ID único (síncrono)
        Product? GetById(int id);

        // Obtiene un producto por ID (versión alternativa que ya usas en tu repo)
        Product? Get(int id);

        // Obtiene un producto por nombre
        Product? Get(string name);

        // Obtiene todos los productos
        List<Product> GetAll();

        // Agrega un nuevo producto y lo devuelve
        Product Add(Product product);

        // Elimina un producto existente
        void Delete(Product product);

        // Actualiza un producto existente (síncrono)
        void Update(Product product);

        // 🔹 NUEVOS MÉTODOS ASÍNCRONOS 🔹

        // Obtiene un producto por su ID de forma asíncrona
        Task<Product?> GetByIdAsync(int id);

        // Actualiza un producto de forma asíncrona
        Task UpdateAsync(Product product);
    }
}

