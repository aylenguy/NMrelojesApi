using Application.Model;
using Application.Model.Request;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAdminServices
    {
        Admin Create(AdminCreateRequest adminCreateRequest);
        void Update(int id, AdminUpdateRequest adminUpdateRequest);
        void Delete(int id);
        List<AdminDto> GetAll();
        List<Admin> GetAllFullData();
        AdminDto GetById(int id);
    }
}
