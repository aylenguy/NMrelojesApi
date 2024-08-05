using Domain.Enums; // Importa el namespace correcto
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Domain.Entities.Domain.Entities;

namespace Infrastructure.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Venta> Ventas { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("UserType")
                .HasValue<Client>("Client")
                .HasValue<Admin>("Admin");

            modelBuilder.Entity<User>()
                .Property(u => u.Rol)
                .HasConversion(new EnumToStringConverter<RolUser>());

            modelBuilder.Entity<Client>()
                .HasMany(c => c.Ventas)
                .WithOne(v => v.Client)
                .HasForeignKey(v => v.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.Ventas)
                .WithOne(v => v.Product)
                .HasForeignKey(v => v.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Admin>().HasData(new Admin
            {
                Id = 1,
                Name = "Admin1",
                LastName = "AdminLast",
                Email = "admin1@example.com",
                Password = "Password1",
                Rol = RolUser.Admin
            });

            modelBuilder.Entity<Client>().HasData(new Client
            {
                Id = 2,
                Name = "Client1",
                LastName = "ClientLast",
                Email = "client1@example.com",
                Password = "Password2",
                Rol = RolUser.Client,
            });

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Shirt",
                    Price = 20,
                    Color = "Black",
                    Stock = 100
                },
                new Product
                {
                    Id = 2,
                    Name = "Shirt",
                    Price = 40,
                    Color = "Blue",
                    Stock = 50
                }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
