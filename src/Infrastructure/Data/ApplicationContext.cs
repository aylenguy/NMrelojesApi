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
                PasswordHash = "$2a$11$/Q98DFfyOZlpeJjmBvNITuxkOoV/PKEEFoYJ8nap1O5VLiGsQq3nu",
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
                    Image = "relojhombre.jpg", // <- cambiamos aquí
                    Description = "Reloj deportivo resistente al agua, ideal para actividades al aire libre.",
                    Color = "Negro",
                    Brand = "Kosiuko",
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

            // ✅ Configuración de DetalleVenta
            modelBuilder.Entity<DetalleVenta>(entity =>
            {
                entity.HasKey(d => d.Id);

                // 👇 Esto fuerza autoincrement
                entity.Property(d => d.Id)
                      .ValueGeneratedOnAdd();

                entity.HasOne(d => d.Venta)
                      .WithMany(v => v.DetalleVentas)
                      .HasForeignKey(d => d.VentaId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Product)
                      .WithMany()
                      .HasForeignKey(d => d.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

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
    .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
