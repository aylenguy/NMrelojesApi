using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class ClientRepository : RepositoryBase<Client>, IClientRepository
    {
        private readonly ApplicationContext _context;
        public ClientRepository(ApplicationContext context) : base(context)
        {
            _context = context;
        }

        public Client? GetByLastName(string lastName)
        {
            return _context.Clients.FirstOrDefault(c => c.LastName == lastName);
        }

    }
}
