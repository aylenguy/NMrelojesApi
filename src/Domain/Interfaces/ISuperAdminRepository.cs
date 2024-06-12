using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ISuperAdminRepository
    {
        SuperAdmin Add(SuperAdmin superAdmin);
        void Update(SuperAdmin superAdmin);
        void Delete(SuperAdmin superAdmin);
        List<SuperAdmin> GetAll();
        SuperAdmin? GetById(int id);
        void SaveChages();
    }
}
