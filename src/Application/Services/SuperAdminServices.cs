using Application.Interfaces;
using Application.Model;
using Application.Model.Request;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class SuperAdminServices : ISuperAdminServices
    {
        private readonly ISuperAdminRepository _superAdminRepository;
        public SuperAdminServices(ISuperAdminRepository superAdminRepository)
        {
            _superAdminRepository = superAdminRepository;
        }

        public void Delete(int id)
        {
            var obj = _superAdminRepository.GetById(id);

            if (obj == null)
                throw new NotFoundException(nameof(SuperAdmin), id);

            _superAdminRepository.Delete(obj);
        }

        public SuperAdminDto GetById(int id)
        {
            var obj = _superAdminRepository.GetById(id)
                ?? throw new NotFoundException(nameof(SuperAdmin), id);

            var dto = SuperAdminDto.Create(obj);

            return dto;
            
        }

        public void Update (int id, SuperAdminUpdateRequest superAdminUpdateRequest)
        {
            var obj = _superAdminRepository.GetById(id);

            if (obj == null)
                throw new NotFoundException(nameof(SuperAdmin), id);

            if (obj.Name != string.Empty) superAdminUpdateRequest.Name = obj.Name;

            if (obj.LastName != string.Empty) superAdminUpdateRequest.LastName = obj.LastName;

            if (obj.Email != string.Empty) superAdminUpdateRequest.Email = obj.Email;

            _superAdminRepository.Update(obj);
        }

        public List<SuperAdminDto> GetAll()
        {
            var obj = _superAdminRepository.GetAll();

            return SuperAdminDto.CreateList(obj);
        }

        public List<SuperAdmin> GetAllFullData()
        {
            return _superAdminRepository.GetAll();
        }

        public SuperAdmin Create(SuperAdminCreateRequest superAdminCreateRequest)
        {
            var obj = new SuperAdmin();
            obj.Name = superAdminCreateRequest.Name;
            obj.LastName = superAdminCreateRequest.LastName;
            obj.Email = superAdminCreateRequest.Email;
            obj.Password = superAdminCreateRequest.Password; //Tengo que chequear si esta
            
            return _superAdminRepository.Add(obj);

        }
    }
}
