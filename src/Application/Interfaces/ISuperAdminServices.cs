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
    public interface ISuperAdminServices
    {
        SuperAdmin Create (SuperAdminCreateRequest superAdminCreateRequest);
        List<SuperAdminDto> GetAll ();
        List<SuperAdmin> GetAllFullData ();
        void Delete (int id);
        void Update (int id, SuperAdminUpdateRequest superAdminUpdateRequest);
        SuperAdminDto GetById (int id);
    }
}
