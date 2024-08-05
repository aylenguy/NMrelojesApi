using Application.Model;
using Application.Model.Request;
using Domain.Entities;
using System.Collections.Generic;

namespace Application.Interfaces
{
    public interface IAdminService
    {
        Admin Create(AdminCreateRequest adminCreateRequest);
        void Update(int id, AdminUpdateRequest adminUpdateRequest);
        void Delete(int id);
        List<AdminDto> GetAll();
        AdminDto GetById(int id);
    }
}
