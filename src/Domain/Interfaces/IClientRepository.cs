using Domain.Entities;
using Domain.Entities.Domain.Entities;
using System.Collections.Generic;

namespace Domain.Interfaces
{
    public interface IClientRepository
    {
        Client Add(Client client);
        void Update(Client client);
        void Delete(Client client);
        List<Client> GetAll();
        Client? GetById(int id);
        void SaveChanges();
    }
}
