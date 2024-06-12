using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IAdminRepository
    {
        Admin Add(Admin admin);
        void Update(Admin admin);
        void Delete(Admin admin);
        List<Admin> GetAll();
        Admin? GetById(int id);
        void SaveChages();
    }
}
