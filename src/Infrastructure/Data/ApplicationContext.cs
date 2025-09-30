using Domain.Entities;
using Microsoft.EntityFrameworkCore;

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

            // Seed de productos
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Reloj Deportivo",
                    Price = 10,
                    OldPrice = 10,
                    Stock = 15,
                    Images = new List<string> { "relojhombre.jpg" },
                    Description = "Reloj deportivo resistente al agua, ideal para actividades al aire libre.",
                    Color = "Negro",
                    Brand = "Kosiuko",
                    Specs = "Cronómetro, GPS, sumergible hasta 50m"
                },
                new Product
                {
                    Id = 2,
                    Name = "Aylen (chico)",
                    Price = 81000,
                    OldPrice = 81000,
                    Stock = 1,
                    Images = new List<string> { "KnockOutAylen.JPEG", "KnockOutAylen2.JPEG" },
                    Description = "-Analógico\r\n-Resistente a salpicaduras WR30\r\n-Fondo nacarado con números plateados\r\n-Caja de plástico\r\n-Tapa de acero\r\n-Malla extensible de metal\r\n-Agujas luminiscentes\r\n-Diámetro: 3,5 cm",
                    Color = "Rose fondo nacarado",
                    Brand = "Knock out",
                    Specs = "Todos los relojes cuentan con 1 año de garantía oficial de fábrica ante cualquier falla en el funcionamiento del reloj"
                },
                new Product
                {
                    Id = 3,
                    Name = "Mica",
                    Price = 110000,
                    OldPrice = 110000,
                    Stock = 1,
                    Images = new List<string> { "KosiukoMica.JPEG", "KosiukoMica2.JPEG" },
                    Description = "Este reloj tiene malla con eslabones, el cual necesita de un relojero para poder ajustarlo, si queres podes dejarnos en comentarios el tamaño de tu muñeca y nos encargamos de enviártelo ajustado listo para usar.\r\n\r\nOtras características:\r\n-Analógico\r\n-Sumergible WR50\r\n-Agujas luminiscentes\r\n-Calendario\r\n-Caja y malla de acero\r\n-Diámetro: 3,8cm",
                    Color = "Dorado",
                    Brand = "Kosiuko",
                    Specs = "Todos los relojes cuentan con 1 año de garantía oficial de fábrica ante cualquier falla en el funcionamiento del reloj"
                },
                new Product
                {
                    Id = 4,
                    Name = "Flor",
                    Price = 110000,
                    OldPrice = 110000,
                    Stock = 1,
                    Images = new List<string> { "relojhombre.jpg" },
                    Description = "Este reloj tiene malla con eslabones...",
                    Color = "Rose",
                    Brand = "Kosiuko",
                    Specs = "Todos los relojes cuentan con 1 año de garantía oficial de fábrica ante cualquier falla en el funcionamiento del reloj"
                },
                new Product
                {
                    Id = 5,
                    Name = "Mela",
                    Price = 69330,
                    OldPrice = 69330,
                    Stock = 1,
                    Images = new List<string> { "relojhombre.jpg" },
                    Description = "-Analógico\r\n-Resistente a salpicaduras: WR30...",
                    Color = "Rose fondo nacarado",
                    Brand = "Knock out",
                    Specs = "Todos los relojes cuentan con 1 año de garantía oficial de fábrica ante cualquier falla en el funcionamiento del reloj"
                },
                new Product
                {
                    Id = 6,
                    Name = "Margot",
                    Price = 93600,
                    OldPrice = 93600,
                    Stock = 1,
                    Images = new List<string> { "relojhombre.jpg" },
                    Description = "-Analógico\r\n-Sumergible 50 mts...",
                    Color = "rosa bb",
                    Brand = "Tressa",
                    Specs = "Todos los relojes cuentan con 1 año de garantía oficial de fábrica ante cualquier falla en el funcionamiento del reloj"
                },
                new Product
                {
                    Id = 7,
                    Name = "Mia",
                    Price = 69331,
                    OldPrice = 69331,
                    Stock = 1,
                    Images = new List<string> { "relojhombre.jpg" },
                    Description = "Este reloj tiene malla con eslabones...",
                    Color = "Plateado",
                    Brand = "Knock out",
                    Specs = "Todos los relojes cuentan con 1 año de garantía oficial de fábrica ante cualquier falla en el funcionamiento del reloj"
                },
                new Product
                {
                    Id = 8,
                    Name = "Abby",
                    Price = 100000,
                    OldPrice = 100000,
                    Stock = 1,
                    Images = new List<string> { "relojhombre.jpg" },
                    Description = "Este reloj tiene malla con eslabones...",
                    Color = "Plateado con nacar",
                    Brand = "Kosiuko",
                    Specs = "Todos los relojes cuentan con 1 año de garantía oficial de fábrica ante cualquier falla en el funcionamiento del reloj"
                },
                new Product
                {
                    Id = 9,
                    Name = "Rita",
                    Price = 60480,
                    OldPrice = 64700,
                    Stock = 1,
                    Images = new List<string> { "relojhombre.jpg" },
                    Description = "-Analógico\r\n-Malla de silicona...",
                    Color = "Blanco",
                    Brand = "Knock out",
                    Specs = "Todos los relojes cuentan con 1 año de garantía oficial de fábrica ante cualquier falla en el funcionamiento del reloj"
                },
                new Product
                {
                    Id = 10,
                    Name = "Génova",
                    Price = 74050,
                    OldPrice = 74050,
                    Stock = 1,
                    Images = new List<string> { "KosiukoGenova.JPG", "KosiukoGenova2.JPG" },
                    Description = "Este reloj tiene malla con eslabones...",
                    Color = "Plateado",
                    Brand = "Knock out",
                    Specs = "Todos los relojes cuentan con 1 año de garantía oficial de fábrica ante cualquier falla en el funcionamiento del reloj"
                },
                new Product
                {
                    Id = 11,
                    Name = "Berlin",
                    Price = 69330,
                    OldPrice = 69330,
                    Stock = 1,
                    Images = new List<string> { "relojhombre.jpg" },
                    Description = "-Analógico\r\n-Resistente a salpicaduras...",
                    Color = "Plateado fondo nacarado",
                    Brand = "Knock out",
                    Specs = "Todos los relojes cuentan con 1 año de garantía oficial de fábrica ante cualquier falla en el funcionamiento del reloj"
                },
                new Product
                {
                    Id = 12,
                    Name = "Paula",
                    Price = 73344,
                    OldPrice = 73344,
                    Stock = 1,
                    Images = new List<string> { "relojhombre.jpg" },
                    Description = "-Analógico\r\n-Resistente a salpicaduras...",
                    Color = "Plateado",
                    Brand = "Knock out",
                    Specs = "Todos los relojes cuentan con 1 año de garantía oficial de fábrica ante cualquier falla en el funcionamiento del reloj"
                },
                new Product
                {
                    Id = 13,
                    Name = "Lucia",
                    Price = 73920,
                    OldPrice = 73920,
                    Stock = 1,
                    Images = new List<string> { "relojhombre.jpg" },
                    Description = "-Analógico\r\n-Resistente al agua W30...",
                    Color = "Nude",
                    Brand = "Tressa",
                    Specs = "Todos los relojes cuentan con 1 año de garantía oficial de fábrica ante cualquier falla en el funcionamiento del reloj"
                },
                new Product
                {
                    Id = 14,
                    Name = "Ari",
                    Price = 93600,
                    OldPrice = 93600,
                    Stock = 1,
                    Images = new List<string> { "relojhombre.jpg" },
                    Description = "-Analógico\r\n-Sumergible 50 mts...",
                    Color = "Tiza con numeros dorados",
                    Brand = "Tressa",
                    Specs = "Todos los relojes cuentan con 1 año de garantía oficial de fábrica ante cualquier falla en el funcionamiento del reloj"
                },
                new Product
                {
                    Id = 15,
                    Name = "Ema",
                    Price = 93600,
                    OldPrice = 93600,
                    Stock = 1,
                    Images = new List<string> { "relojhombre.jpg" },
                    Description = "-Analógico\r\n-Sumergible 50 mts...",
                    Color = "Blanco fondo dorado",
                    Brand = "Tressa",
                    Specs = ""
                },
                new Product
                {
                    Id = 16,
                    Name = "Naomi",
                    Price = 89700,
                    OldPrice = 89700,
                    Stock = 1,
                    Images = new List<string> { "relojhombre.jpg" },
                    Description = "– Analógico\r\n– Resistencia al agua...",
                    Color = "Negro",
                    Brand = "Kosiuko",
                    Specs = ""
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

            // Configuración de DetalleVenta
            modelBuilder.Entity<DetalleVenta>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.Property(d => d.Id).ValueGeneratedOnAdd();

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
