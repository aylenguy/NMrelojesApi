using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetalleVentas { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("UserType")
                .HasValue<Client>("Client")
                .HasValue<Admin>("Admin");

            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    LastName = "Guy",
                    Name = "Aylen",
                    Email = "aylenguy@gmail.com",
                    UserName = "aylu",
                    Password = "123",
                    Id = 5,
                    UserType = "Admin"
                });

            modelBuilder.Entity<Client>().HasData(
                new Client
                {
                    LastName = "Fernandez",
                    Name = "Matias",
                    Email = "matiasfernandez@gmail.com",
                    UserName = "mati",
                    Password = "1234",
                    Address = "rueda 2217",
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

            // Relación entre Cliente y Venta (uno a muchos)
            modelBuilder.Entity<Client>()
                .HasMany(c => c.Ventas)
                .WithOne(o => o.Client)
                .HasForeignKey(o => o.ClientId)
                .OnDelete(DeleteBehavior.Cascade); // Al eliminar un Cliente, se eliminan todos los Ventas asociados.

            // Relación entre Venta y DetalleVenta (uno a muchos)
            modelBuilder.Entity<Venta>()
                .HasMany(o => o.DetalleVentas)
                .WithOne(l => l.Venta)
                .HasForeignKey(l => l.VentaId)
                .OnDelete(DeleteBehavior.Cascade); // Al eliminar una Venta, se eliminan todos los DetalleVentas asociados.

            // Relación entre DetalleVenta y Producto (muchos a uno)
            modelBuilder.Entity<DetalleVenta>()
                .HasOne(sol => sol.Product)
                .WithMany() // Vacío porque no me interesa establecer esa relación inversa
                .HasForeignKey(sol => sol.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // Al intentar eliminar un Product, no se podrá si tiene alguna DetalleVenta que lo referencie.

            base.OnModelCreating(modelBuilder);
        }
    }
}
