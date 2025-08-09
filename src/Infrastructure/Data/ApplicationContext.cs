using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure.Data
{
    // Esta clase representa el contexto principal de EF Core.
    // Acá se configuran las tablas (DbSet) y las relaciones con Fluent API.
    public class ApplicationContext : DbContext
    {
        // Cada DbSet representa una tabla en la base de datos.
        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetalleVentas { get; set; }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        //--- Fluent API ---
        // Se usa para configurar cómo se mapearán las entidades a tablas y establecer relaciones, constraints y datos iniciales.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración para herencia en Users
            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("UserType")
                .HasValue<Client>("Client")
                .HasValue<Admin>("Admin");

            // Seed de usuario administrador
            modelBuilder.Entity<Admin>().HasData(new Admin
            {
                Id = 1,
                Email = "aylenguy@gmail.com",
                Name = "Aylen",
                LastName = "Guy",
                UserName = "aylen",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123"),
                UserType = "Admin"
            });

            // Seed de producto inicial
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Reloj Deportivo",
                    Price = 105000,
                    OldPrice = 120000,
                    Stock = 15,
                    Image = "reloj-deportivo.jpg",
                    Description = "Reloj deportivo resistente al agua, ideal para actividades al aire libre.",
                    Color = "Negro",
                    Specs = "Cronómetro, GPS, sumergible hasta 50m"
                }
            );

            // Relación Client -> Venta (1:N)
            modelBuilder.Entity<Client>()
                .HasMany(c => c.Ventas)
                .WithOne(o => o.Client)
                .HasForeignKey(o => o.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación Venta -> DetalleVenta (1:N)
            modelBuilder.Entity<Venta>()
                .HasMany(o => o.DetalleVentas)
                .WithOne(l => l.Venta)
                .HasForeignKey(l => l.VentaId)
                .OnDelete(DeleteBehavior.Cascade);

            // DetalleVenta -> Product (N:1) con restricción para no borrar producto si hay ventas
            modelBuilder.Entity<DetalleVenta>()
                .HasOne(d => d.Product)
                .WithMany()
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Cart -> CartItems (1:N)
            modelBuilder.Entity<Cart>()
                .HasMany(c => c.Items)
                .WithOne(i => i.Cart)
                .HasForeignKey(i => i.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            // CartItem -> Product (N:1) con restricción
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}

