using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class ClientRepository : IClientRepository
    {
        private readonly ApplicationContext _context;

        public ClientRepository(ApplicationContext context)
        {
            _context = context;
        }

        public User? GetClientByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email && u.UserType == "Client");
        }

        public void Add(User client)
        {
            _context.Users.Add(client);
            _context.SaveChanges();
        }
    }
}

