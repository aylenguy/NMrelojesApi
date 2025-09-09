using Application.Interfaces;
using Application.Models.Requests;
using Domain.Entities;
using Domain.Interfaces;
using System.Collections.Generic;

namespace Infrastructure.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _repository;
        private readonly IUserRepository _userRepository;

        public AdminService(IAdminRepository repository, IUserRepository userRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
        }

        // ---------------------- Admin ----------------------
        public List<Admin> GetAllAdmins() => _repository.Get();

        public Admin? Get(int id) => _repository.Get(id);

        public Admin? Get(string name) => _repository.Get(name);

        public Admin? GetByEmail(string email) => _repository.GetByEmail(email);

        public int AddAdmin(AdminCreateRequest request)
        {
            var admin = new Admin()
            {
                Name = request.Name,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.UserName,
                UserType = "Admin"
            };
            return _repository.Add(admin).Id;
        }

        public void UpdateAdmin(int id, AdminUpdateRequest request)
        {
            var adminToUpdate = _repository.Get(id);
            if (adminToUpdate == null) return;

            adminToUpdate.Name = request.Name;
            adminToUpdate.LastName = request.LastName;
            adminToUpdate.Email = request.Email;
            adminToUpdate.UserName = request.UserName;

            _repository.Update(adminToUpdate);
        }

        public void DeleteAdmin(int id)
        {
            var adminToDelete = _repository.Get(id);
            if (adminToDelete == null) return;

            _repository.Delete(adminToDelete);
        }

        // ---------------------- Users ----------------------
        public IEnumerable<User> GetAllUsers() => _userRepository.GetAll();

        public bool DeleteUser(int id)
        {
            var user = _userRepository.GetById(id);
            if (user == null) return false;

            _userRepository.Delete(user);
            return true;
        }

        public bool UpdateUserRole(int id, string userType)
        {
            var user = _userRepository.GetById(id);
            if (user == null) return false;

            // Aquí NO usamos Update(user) para evitar InvalidOperationException
            user.UserType = userType;
            _userRepository.SaveChanges(); // Nuevo método para guardar cambios
            return true;
        }
    }
}
