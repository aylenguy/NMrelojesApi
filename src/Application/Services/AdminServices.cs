using Application.Interfaces;
using Application.Model;
using Application.Model.Request;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using System.Collections.Generic;

namespace Application.Services
{
    public class AdminServices : IAdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminServices(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public AdminDto GetById(int id)
        {
            var obj = _adminRepository.GetById(id)
                      ?? throw new NotFoundException(nameof(Admin), id);
            return AdminDto.Create(obj);
        }

        public List<AdminDto> GetAll()
        {
            var list = _adminRepository.GetAll();
            return AdminDto.CreateList(list);
        }

        public List<Admin> GetAllFullData()
        {
            return _adminRepository.GetAll();
        }

        public Admin Create(AdminCreateRequest adminCreateRequest)
        {
            var admin = new Admin
            {
                Name = adminCreateRequest.Name,
                Email = adminCreateRequest.Email,
                Password = adminCreateRequest.Password,
                LastName = adminCreateRequest.LastName
            };

            return _adminRepository.Add(admin);
        }

        public void Update(int id, AdminUpdateRequest adminUpdateRequest)
        {
            var admin = _adminRepository.GetById(id)
                        ?? throw new NotFoundException(nameof(Admin), id);

            if (!string.IsNullOrWhiteSpace(adminUpdateRequest.Name)) admin.Name = adminUpdateRequest.Name;
            if (!string.IsNullOrWhiteSpace(adminUpdateRequest.LastName)) admin.LastName = adminUpdateRequest.LastName;
            if (!string.IsNullOrWhiteSpace(adminUpdateRequest.Email)) admin.Email = adminUpdateRequest.Email;

            _adminRepository.Update(admin);
        }

        public void Delete(int id)
        {
            var admin = _adminRepository.GetById(id)
                        ?? throw new NotFoundException(nameof(Admin), id);
            _adminRepository.Delete(admin);
        }
    }
}
