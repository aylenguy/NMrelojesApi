using Application.Interfaces;
using Application.Model;
using Application.Model.Request;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AdminServices : IAdminServices
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

            var dto = AdminDto.Create(obj);

            return dto;

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

        public Admin Create (AdminCreateRequest adminCreateRequest) 
        {
            var obj = new Admin();
            obj.Name = adminCreateRequest.Name;
            obj.Email = adminCreateRequest.Email;
            obj.Password = adminCreateRequest.Password;
            obj.LastName = adminCreateRequest.LastName;

            return _adminRepository.Add(obj);
        }

        public void Update(int id, AdminUpdateRequest adminUpdateRequest)
        {
            var obj = _adminRepository.GetById(id);

            if (obj == null)
                throw new NotFoundException(nameof(Admin), id);

            if (adminUpdateRequest.Name != string.Empty) obj.Name = adminUpdateRequest.Name;

            if (adminUpdateRequest.LastName != string.Empty) obj.LastName = adminUpdateRequest.LastName;

            if (adminUpdateRequest.Email != string.Empty) obj.Email = adminUpdateRequest.Email;

            _adminRepository.Update(obj);
        }

        public void Delete(int id)
        {
            var obj = _adminRepository.GetById(id);

            if (obj == null)
                throw new NotFoundException(nameof(Admin), id);


            _adminRepository.Delete(obj);
        }

    }
}
