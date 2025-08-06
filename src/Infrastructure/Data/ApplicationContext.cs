using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;


//aca interactúan las entidades con la base de datos.
namespace Infrastructure.Data
{
    public class ApplicationContext : DbContext
    {
        //dbset recibe una entidad y la representa en una tabla en la base de datos
        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Venta> Ventas { get; set; } 
        public DbSet<DetalleVenta> DetalleVentas { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {

        }
        //---Fuent API---
        // Este método (OnModelCreating) se usa para definir cómo se deben mapear las entidades del dominio a las tablas de la base de datos
        // y establecer sus relaciones, conversiones (el enum a cadena) y datos iniciales (Seed Data).
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("UserType")
                .HasValue<Client>("Client")
                .HasValue<Admin>("Admin");

            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    Name = "Aylen",
                    LastName = "Guy",
                    Email = "aylenguy@gmail.com",
                    UserName = "aylu",
                    Password = "123",
                    Id = 5,
                    UserType = "Admin"
                });

            modelBuilder.Entity<Client>().HasData(
                new Client
                {
                    Name = "Matias",
                    LastName = "Fernandez",
                    Email = "matiasfernandez@gmail.com",
                    UserName = "mati",
                    PhoneNumber = "341678345",
                    Password = "1234",
                    Id = 3,
                    UserType = "Client"
                });

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "remera",
                    Price = 10500,
                    Stock = 10
                });

            // Relación entre Cliente y Venta (uno a muchos). Client tiene muchas Ventas y una Venta pertenece a un Client
            modelBuilder.Entity<Client>()
                .HasMany(c => c.Ventas)
                .WithOne(o => o.Client)
                .HasForeignKey(o => o.ClientId)
                .OnDelete(DeleteBehavior.Cascade); // Si se elimina un Cliente, se eliminan todos los Ventas asociados.

            // Relación entre Venta y DetalleVenta (uno a muchos). Venta tiene muchos DetalleVentas y un DetalleVenta pertenece a una Venta
            modelBuilder.Entity<Venta>()
                .HasMany(o => o.DetalleVentas)
                .WithOne(l => l.Venta)
                .HasForeignKey(l => l.VentaId)
                .OnDelete(DeleteBehavior.Cascade); // SI se elimina una Venta, se eliminan todos los DetalleVentas asociados.

            //  DetalleVenta pertenece a un Product, pero no se elimina el Product si tiene DetalleVentas que lo referencian 
            modelBuilder.Entity<DetalleVenta>()
                .HasOne(sol => sol.Product)
                .WithMany() 
                .HasForeignKey(sol => sol.ProductId)
                .OnDelete(DeleteBehavior.Restrict); 

            base.OnModelCreating(modelBuilder);
        }
    }
}
