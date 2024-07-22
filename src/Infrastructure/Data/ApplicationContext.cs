using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<SuperAdmin> SuperAdmins { get; set; }
        public DbSet<User> Users { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasDiscriminator<Rol>("Role")
                .HasValue<Admin>(Rol.Admin)
                .HasValue<Client>(Rol.Client)
                .HasValue<SuperAdmin>(Rol.SuperAdmin);

            // Configurar las propiedades de enum para almacenarse como cadenas
            modelBuilder.Entity<User>()
                .Property(u => u.Rol)
                .HasConversion(new EnumToStringConverter<Rol>());

            // Seed Data para los usuarios
            modelBuilder.Entity<Admin>().HasData(new Admin
            {
                Id = 1,
                Name = "Admin1",
                LastName = "AdminLast",
                Email = "admin1@example.com",
                Password = "Password1"
            });

            modelBuilder.Entity<Client>().HasData(new Client
            {
                Id = 2,
                Name = "Client1",
                LastName = "ClientLast",
                Email = "client1@example.com",
                Password = "Password2"
            });

            modelBuilder.Entity<SuperAdmin>().HasData(new SuperAdmin
            {
                Id = 3,
                Name = "SuperAdmin1",
                LastName = "SuperLast",
                Email = "superadmin1@example.com",
                Password = "Password3"
            });

            // Seed Data para los productos
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Shirt",
                    Price = 20,
                    Size = "M",
                    Color = "Red",
                    Description = "Red Shirt",
                    PhotoUrl = "http://example.com/shirt.jpg"
                },
                new Product
                {
                    Id = 2,
                    Name = "Jeans",
                    Price = 40,
                    Size = "L",
                    Color = "Blue",
                    Description = "Blue Jeans",
                    PhotoUrl = "http://example.com/jeans.jpg"
                }
            );
        }
    }
}
