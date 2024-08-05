using Domain.Entities;
using System.Collections.Generic;

namespace Domain.Interfaces
{
    public interface IAdminRepository
    {
        Admin Add(Admin admin);
        void Update(Admin admin);
        void Delete(Admin admin);
        List<Admin> GetAll();
        Admin? GetById(int id);
        void SaveChanges();
    }
}
