using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class SuperAdminRepositoryEf : ISuperAdminRepository
    {
        private readonly ApplicationContext _context;

        public SuperAdminRepositoryEf(ApplicationContext context)
        {
            _context = context;
        }

        public SuperAdmin Add(SuperAdmin superAdmin)
        {
            _context.SuperAdmin.Add(superAdmin);
            _context.SaveChanges();
            return superAdmin;
        }

        public void Delete(SuperAdmin superAdmin)
        {
            _context.Remove(superAdmin);
            _context.SaveChanges();

        }

        public List<SuperAdmin> GetAll()
        {
            return _context.SuperAdmin.ToList();
        }

        public SuperAdmin? GetById(int id)
        {
            return _context.SuperAdmin
                .FirstOrDefault(x => x.Id == id);
        }

        public void Update(SuperAdmin superAdmin)
        {
            _context.Update(superAdmin);
            _context.SaveChanges();

        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
