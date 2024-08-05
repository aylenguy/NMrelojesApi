using Domain.Entities;
using Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Data
{
    public class AdminRepositoryEf : IAdminRepository
    {
        private readonly ApplicationContext _context;

        public AdminRepositoryEf(ApplicationContext context)
        {
            _context = context;
        }

        public Admin Add(Admin admin)
        {
            _context.Admins.Add(admin); 
            _context.SaveChanges();
            return admin;
        }

        public void Delete(Admin admin)
        {
            _context.Admins.Remove(admin);
            _context.SaveChanges();
        }

        public List<Admin> GetAll()
        {
            return _context.Admins.ToList(); 
        }

        public Admin? GetById(int id)
        {
            return _context.Admins
                .FirstOrDefault(x => x.Id == id); 
        }

        public void Update(Admin admin)
        {
            _context.Admins.Update(admin); 
            _context.SaveChanges();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}

