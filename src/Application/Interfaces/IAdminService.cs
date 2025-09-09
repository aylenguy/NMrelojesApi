using Domain.Entities;
using Application.Models.Requests;
using System.Collections.Generic;

namespace Application.Interfaces
{
    public interface IAdminService
    {
        // Admin
        List<Admin> GetAllAdmins();
        Admin? Get(int id);
        Admin? Get(string name);
        Admin? GetByEmail(string email);
        int AddAdmin(AdminCreateRequest request);
        void UpdateAdmin(int id, AdminUpdateRequest request);
        void DeleteAdmin(int id);

        // Users
        IEnumerable<User> GetAllUsers();
        bool DeleteUser(int id);
        bool UpdateUserRole(int id, string userType);
    }
}
