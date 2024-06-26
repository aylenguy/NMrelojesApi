using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            _context.Admin.Add(admin);
            _context.SaveChanges();
            return admin;
        }

        public void Delete(Admin admin)
        {
            _context.Remove(admin);
            _context.SaveChanges();

        }

        public List<Admin> GetAll()
        {
            return _context.Admin.ToList();
        }

        public Admin? GetById(int id)
        {
            return _context.Admin
                .FirstOrDefault(x => x.Id == id);
        }

        public void Update(Admin admin)
        {
            _context.Update(admin);
            _context.SaveChanges();

        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
