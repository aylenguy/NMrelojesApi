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
    public class ClientServices : IClientServices
    {
        private readonly IClientRepository _clientRepository;

        public ClientServices(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public void Update (int id, ClientUpdatedRequest clientUpdatedRequest)
        {
            var obj = _clientRepository.GetById(id);

            if (obj == null)
                throw new NotFoundException(nameof(Client), id);

            if (obj.Name != string.Empty) clientUpdatedRequest.Name = obj.Name;
            
            if (obj.LastName != string.Empty) clientUpdatedRequest.LastName = obj.LastName;

            if (obj.Email != string.Empty) clientUpdatedRequest.Email = obj.Email;

            _clientRepository.Update(obj);
        }


        public void Delete (int id)
        {
            var obj = _clientRepository.GetById (id)
                ?? throw new NotFoundException(nameof(Client), id);

            _clientRepository.Delete(obj);
        }

        public List<ClientDto> GetAll()
        {
            var list = _clientRepository.GetAll();

            return ClientDto.CreateList(list);
        }

        public ClientDto GetById (int id)
        {
            var obj = _clientRepository.GetById(id)
                ?? throw new NotFoundException(nameof(Client),id);

            var dto = ClientDto.Create(obj);

            return dto;
        }

        public List<Client> GetAllFullData ()
        {
            return _clientRepository.GetAll();
        }

        public Client Create (ClientCreateRequest clientCreateRequest)
        {
            var client = new Client();
            client.Name = clientCreateRequest.Name;
            client.LastName = clientCreateRequest.LastName;
            client.Email = clientCreateRequest.Email;
            client.Password = clientCreateRequest.Password;

            return _clientRepository.Add(client);
        }

    }
}
