using Domain.Entities;
using System.Collections.Generic;

namespace Application.Interfaces
{
    public interface IUserRepository
    {
        IEnumerable<User> GetAll();
        User? GetById(int id);
        User? GetByName(string name);
        User? GetUserByEmail(string email); // <-- Agregar esta línea
        void Add(User user);
        void Update(User user);
        void Delete(User user);

        void SaveChanges();
    }
}
