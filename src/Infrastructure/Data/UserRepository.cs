using Domain.Entities;
using Application.Interfaces; // 👈 importante: implementar la interfaz correcta
using System.Collections.Generic;
using Application.Model;
using System.Linq;

namespace Infrastructure.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationContext _context;

        public UserRepository(ApplicationContext context)
        {
            _context = context;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users.ToList();
        }

        public User? GetById(int id)
        {
            return _context.Users.Find(id);
        }

        public User? GetUserByEmail(string email) // 👈 Método agregado
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }
        public User? GetByName(string name)
        {
            return _context.Users.FirstOrDefault(u => u.UserName == name);
        }

        public void Add(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
        public void Delete(User user)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }
    }
}
