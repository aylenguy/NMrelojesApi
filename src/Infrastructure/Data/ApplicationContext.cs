using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Admin> Admin { get; set; }

        public DbSet<SuperAdmin> SuperAdmin { get; set; }

        public DbSet<Client> Client { get; set; }

        public DbSet<Product> Product { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {

        }
    }
}
