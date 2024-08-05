using Application.Model;
using Application.Model.Request;
using Domain.Entities;
using Domain.Entities.Domain.Entities;
using System.Collections.Generic;

namespace Application.Interfaces
{
    public interface IClientService
    {
        Client Create(ClientCreateRequest clientCreateRequest);
        void Delete(int id);
        void Update(int id, ClientUpdateRequest clientUpdateRequest);
        ClientDto GetById(int id);
        List<ClientDto> GetAll();
        
    }
}
