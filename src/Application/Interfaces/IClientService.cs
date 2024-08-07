using Application.Models.Requests;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IClientService
    {
        List<Client> GetAllClients();
        Client? Get(int id);
        Client? GetByLastName(string lastName);
        int AddClient(ClientCreateRequest request);
        void DeleteClient(int id);
        void UpdateClient(int id, ClientUpdateRequest request);
    }
}
