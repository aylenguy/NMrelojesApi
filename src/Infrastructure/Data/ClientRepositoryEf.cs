using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            _context.Client.Add(client);
            _context.SaveChanges();
            return client;
        }

        public void Delete(Client client)
        {
            _context.Remove(client);
            _context.SaveChanges();

        }

        public List<Client> GetAll()
        {
            return _context.Client.ToList();
        }

        public Client? GetById(int id)
        {
            return _context.Client
                .FirstOrDefault(x => x.Id == id);
        }

        public void Update(Client client)
        {
            _context.Update(client);
            _context.SaveChanges();

        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
