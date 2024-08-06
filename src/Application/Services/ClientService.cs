using Application.Interfaces;
using Application.Models.Requests;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _repository;
        public ClientService(IClientRepository repository)
        {
            _repository = repository;
        }

        public List<Client> GetAllClients()
        {
            return _repository.Get();
        }

        public Client? Get(int id)
        {
            return _repository.Get(id);
        }

        public Client? Get(string name)
        {
            return _repository.Get(name);
        }

        public int AddClient(ClientCreateRequest request)
        {
            var client = new Client()
            {
                Name = request.Name,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.UserName,
                Password = request.Password,
                UserType = "Client",
                Address = request.Address,
            };
            return _repository.Add(client).Id;
        }

        public void DeleteClient(int id)
        {
            var clientToDelete = _repository.Get(id);
            if (clientToDelete != null)
            {
                _repository.Delete(clientToDelete);
            }
        }

        public void UpdateClient(int id, ClientUpdateRequest request)
        {
            var clientToUpdate = _repository.Get(id);
            if (clientToUpdate != null)

            {

                clientToUpdate.Email = request.Email;
                clientToUpdate.UserName = request.UserName;
                clientToUpdate.Password = request.Password;
                clientToUpdate.Address = request.Address;
                clientToUpdate.Name = request.Name;
                clientToUpdate.LastName = request.LastName;

                _repository.Update(clientToUpdate);
            }
        }
    }
}