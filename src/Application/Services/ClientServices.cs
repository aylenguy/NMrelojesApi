using Application.Interfaces;
using Application.Model;
using Application.Model.Request;
using Domain.Entities;
using Domain.Entities.Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using System.Collections.Generic;

namespace Application.Services
{
    public class ClientServices : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientServices(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public ClientDto GetById(int id)
        {
            var client = _clientRepository.GetById(id)
                         ?? throw new NotFoundException(nameof(Client), id);
            return ClientDto.Create(client);
        }

        public List<ClientDto> GetAll()
        {
            var list = _clientRepository.GetAll();
            return ClientDto.CreateList(list);
        }

        public List<Client> GetAllFullData()
        {
            return _clientRepository.GetAll();
        }

        public Client Create(ClientCreateRequest clientCreateRequest)
        {
            var client = new Client
            {
                Name = clientCreateRequest.Name,
                LastName = clientCreateRequest.LastName,
                Email = clientCreateRequest.Email,
                Password = clientCreateRequest.Password
            };

            return _clientRepository.Add(client);
        }

        public void Update(int id, ClientUpdateRequest clientUpdateRequest)
        {
            var client = _clientRepository.GetById(id)
                         ?? throw new NotFoundException(nameof(Client), id);

            if (!string.IsNullOrWhiteSpace(clientUpdateRequest.Name)) client.Name = clientUpdateRequest.Name;
            if (!string.IsNullOrWhiteSpace(clientUpdateRequest.LastName)) client.LastName = clientUpdateRequest.LastName;
            if (!string.IsNullOrWhiteSpace(clientUpdateRequest.Email)) client.Email = clientUpdateRequest.Email;

            _clientRepository.Update(client);
        }

        public void Delete(int id)
        {
            var client = _clientRepository.GetById(id)
                         ?? throw new NotFoundException(nameof(Client), id);
            _clientRepository.Delete(client);
        }
    }
}
