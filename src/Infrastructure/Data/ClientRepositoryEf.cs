using Domain.Entities;
using Domain.Entities.Domain.Entities;
using Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Data
{
    public class ClientRepositoryEf : IClientRepository
    {
        private readonly ApplicationContext _context;

        public ClientRepositoryEf(ApplicationContext context)
        {
            _context = context;
        }

        public Client Add(Client client)
        {
            _context.Clients.Add(client);
            _context.SaveChanges();
            return client;
        }

        public void Delete(Client client)
        {
            _context.Clients.Remove(client);
            _context.SaveChanges();
        }

        public List<Client> GetAll()
        {
            return _context.Clients.ToList();
        }

        public Client? GetById(int id)
        {
            return _context.Clients.FirstOrDefault(x => x.Id == id);
        }

        public void Update(Client client)
        {
            _context.Clients.Update(client);
            _context.SaveChanges();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
