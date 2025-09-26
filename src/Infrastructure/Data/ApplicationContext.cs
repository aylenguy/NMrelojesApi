using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

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

            // ✅ Conversor para guardar List<string> como string separado por comas
            var splitStringConverter = new ValueConverter<List<string>, string>(
        v => string.Join(',', v),
        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
    );

            // ✅ Comparador para que EF detecte cambios en la lista
            var listComparer = new ValueComparer<List<string>>(
                (c1, c2) => c1.SequenceEqual(c2), // igualdad elemento por elemento
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())), // hash
                c => c.ToList() // clonación
            );

            modelBuilder.Entity<Product>()
                .Property(p => p.Images)
                .HasConversion(splitStringConverter)
                .Metadata.SetValueComparer(listComparer);

            // Seed de producto inicial
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
                }
            );

            modelBuilder.Entity<Product>().HasData(
              new Product
              {
                  Id = 2,
                  Name = "Aylen (chico)",
                  Price = 81000,
                  OldPrice = 81000,
                  Stock = 1,
                  Images = new List<string> { "KnockOutAylen.JPEG", "KnockOut2.JPEG" },
                  Description = "-Analógico\r\n-Resistente a salpicaduras WR30\r\n-Fondo nacarado con números plateados\r\n-Caja de plástico\r\n-Tapa de acero\r\n-Malla extensible de metal\r\n-Agujas luminiscentes\r\n-Diámetro: 3,5 cm",
                  Color = "Rose fondo nacarado",
                  Brand = "Knock out",
                  Specs = "Todos los relojes cuentan con 1 año de garantía oficial de fábrica ante cualquier falla en el funcionamiento del reloj"
              }



          );

            modelBuilder.Entity<Product>().HasData(
              new Product
              {
                  Id = 3,
                  Name = "Mica",
                  Price = 110000,
                  OldPrice = 110000,
                  Stock = 1,
                  Images = new List<string> { "KosiukoMica.JPEG", "kosiukoMica2.JPEG" }, // <- cambiamos aquí
                  Description = "Este reloj tiene malla con eslabones, el cual necesita de un relojero para poder ajustarlo, si queres podes dejarnos en comentarios el tamaño de tu muñeca y nos encargamos de enviártelo ajustado listo para usar.\r\n\r\nOtras características:\r\n-Analógico\r\n-Sumergible WR50\r\n-Agujas luminiscentes\r\n-Calendario\r\n-Caja y malla de acero\r\n-Diámetro: 3,8cm",
                  Color = "Dorado",
                  Brand = "Kosiuko",
                  Specs = "Todos los relojes cuentan con 1 año de garantía oficial de fábrica ante cualquier falla en el funcionamiento del reloj"
              }





          );

            modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 4,
                Name = "Flor",
                Price = 110000,
                OldPrice = 110000,
                Stock = 1,
                Images = new List<string> { "relojhombre.jpg" },
                Description = "Este reloj tiene malla con eslabones, el cual necesita de un relojero para poder ajustarlo, si queres podes dejarnos en comentarios el tamaño de tu muñeca y nos encargamos de enviártelo ajustado listo para usar.\r\n \r\nOtras características:\r\n-Analógico\r\n-Sumergible WR50\r\n-Agujas luminiscentes\r\n-Calendario\r\n-Caja y malla de acero\r\n-Diámetro: 3,8cm",
                Color = "Rose",
                Brand = "Kosiuko",
                Specs = "Todos los relojes cuentan con 1 año de garantía oficial de fábrica ante cualquier falla en el funcionamiento del reloj"
            }





        );
            modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 5,
                Name = "Mela",
                Price = 69330,
                OldPrice = 69330,
                Stock = 1,
                Images = new List<string> { "relojhombre.jpg" },
                Description = "-Analógico\r\n-Resistente a salpicaduras: WR30\r\n-Fondo nacarado\r\n-Caja de metal\r\n-Malla tejida regulable de metal\r\n-Cierre autoajustable de acero\r\n-Agujas luminiscentes\r\n-Diámetro: 3,4 cm\r\n",
                Color = "Rose fondo nacarado",
                Brand = "Knock out",
                Specs = "Todos los relojes cuentan con 1 año de garantía oficial de fábrica ante cualquier falla en el funcionamiento del reloj"
            }





        );
            modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 6,
                Name = "Margot",
                Price = 93600,
                OldPrice = 93600,
                Stock = 1,
                Images = new List<string> { "relojhombre.jpg" },
                Description = "-Analógico\r\n-Sumergible 50 mts\r\n-Caja de acrílico\r\n-Tapa de acero\r\n-Malla de silicona perlada\r\n-Diámetro: 3,5cm",
                Color = "rosa bb",
                Brand = "Tressa",
                Specs = "Todos los relojes cuentan con 1 año de garantía oficial de fábrica ante cualquier falla en el funcionamiento del reloj"
            }





        );
            modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 7,
                Name = "Mia",
                Price = 69331,
                OldPrice = 69331,
                Stock = 1,
                Images = new List<string> { "relojhombre.jpg" },
                Description = "Este reloj tiene malla con eslabones, el cual necesita de un relojero para poder ajustarlo, si queres podes dejarnos en comentarios el tamaño de tu muñeca y nos encargamos de enviártelo ajustado listo para usar.\r\n\r\nOtras características:\r\n-Analógico\r\n-Resistente al agua W30\r\n-Fondo nacarado con perlitas\r\n-Agujas luminiscentes\r\n-Caja y malla de metal\r\n-Tapa y cierre de acero\r\n-Diámetro: 3cm",
                Color = "Plateado",
                Brand = "Knock out",
                Specs = "Todos los relojes cuentan con 1 año de garantía oficial de fábrica ante cualquier falla en el funcionamiento del reloj"
            }


        );

            modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 8,
                Name = "Abby",
                Price = 100000,
                OldPrice = 100000,
                Stock = 1,
                Images = new List<string> { "relojhombre.jpg" },
                Description = "Este reloj tiene malla con eslabones, el cual necesita de un relojero para poder ajustarlo, si queres podes dejarnos en comentarios el tamaño de tu muñeca y nos encargamos de enviártelo ajustado listo para usar.\r\n\r\nOtras características:\r\n-Analógico\r\n-Sumergible 50 mts\r\n-Calendario\r\n-Agujas luminiscentes\r\n-Caja de acetato y acero\r\n-Tapa y cierre de acero\r\n-Ancho pulsera: 2,2cm\r\n-Diámetro: 4 cm\r\n",
                Color = "Plateado con nacar",
                Brand = "Kosiuko",
                Specs = "Todos los relojes cuentan con 1 año de garantía oficial de fábrica ante cualquier falla en el funcionamiento del reloj"
            }


        );

            modelBuilder.Entity<Product>().HasData(
               new Product
               {
                   Id = 9,
                   Name = "Rita",
                   Price = 60480,
                   OldPrice = 64700,
                   Stock = 1,
                   Images = new List<string> { "relojhombre.jpg" },
                   Description = "-Analógico\r\n-Malla de silicona sin glitter\r\n-Resistente al agua W30\r\n-Caja de plástico ABS\r\n-Tapa de acero\r\n-Hebilla de plástico\r\n-Diámetro del reloj : 4 cm",
                   Color = "Blanco",
                   Brand = "Knock out",
                   Specs = "Todos los relojes cuentan con 1 año de garantía oficial de fábrica ante cualquier falla en el funcionamiento del reloj"
               }


           );

            modelBuilder.Entity<Product>().HasData(
           new Product
           {
               Id = 10,
               Name = "Génova",
               Price = 74050,
               OldPrice = 74050,
               Stock = 1,
               Images = new List<string> { "KosiukoGenova.JPG", "KosiukoGenova2.JPG" },
               Description = "Este reloj tiene malla con eslabones, el cual necesita de un relojero para poder ajustarlo, si queres podes dejarnos en comentarios el tamaño de tu muñeca y nos encargamos de enviártelo ajustado listo para usar.\r\n\r\nOtras características:\r\n-Analógico\r\n-Resistente a salpicaduras WR30\r\n-Caja de metal\r\n-Tapa y cierre de acero\r\n-Malla combinada\r\n-Strass\r\n-Agujas luminiscentes\r\n-Diámetro: 3,4cm",
               Color = "Plateado",
               Brand = "Knock out",
               Specs = "Todos los relojes cuentan con 1 año de garantía oficial de fábrica ante cualquier falla en el funcionamiento del reloj"
           }


       );

            modelBuilder.Entity<Product>().HasData(
           new Product
           {
               Id = 11,
               Name = "Berlin",
               Price = 69330,
               OldPrice = 69330,
               Stock = 1,
               Images = new List<string> { "relojhombre.jpg" },
               Description = "-Analógico\r\n-Resistente a salpicaduras: WR30\r\n-Fondo nacarado\r\n-Caja de metal\r\n-Malla tejida regulable de metal\r\n-Cierre autoajustable de acero\r\n-Agujas luminiscentes\r\n-Diámetro: 3,4 cm",
               Color = "Plateado fondo nacarado",
               Brand = "Knock out",
               Specs = "Todos los relojes cuentan con 1 año de garantía oficial de fábrica ante cualquier falla en el funcionamiento del reloj"
           }


       );
            modelBuilder.Entity<Product>().HasData(
          new Product
          {
              Id = 12,
              Name = "Paula",
              Price = 73344,
              OldPrice = 73344,
              Stock = 1,
              Images = new List<string> { "relojhombre.jpg" },
              Description = "-Analógico\r\n-Resistente a salpicaduras WR30\r\n-Caja de metal\r\n-Tapa de acero\r\n-Malla extensible de metal\r\n-Diámetro: 3,3cm",
              Color = "Plateado",
              Brand = "Knock out",
              Specs = "Todos los relojes cuentan con 1 año de garantía oficial de fábrica ante cualquier falla en el funcionamiento del reloj"
          }


      );
            modelBuilder.Entity<Product>().HasData(
         new Product
         {
             Id = 13,
             Name = "Lucia",
             Price = 73920,
             OldPrice = 73920,
             Stock = 1,
             Images = new List<string> { "relojhombre.jpg" },
             Description = "-Analógico\r\n-Resistente al agua W30\r\n-Malla de silicona sin glitter\r\n-Caja de plástico ABS\r\n-Tapa de acero\r\n-Hebilla de plástico\r\n-Diámetro del reloj : 4,2  cm",
             Color = "Nude",
             Brand = "Tressa",
             Specs = "Todos los relojes cuentan con 1 año de garantía oficial de fábrica ante cualquier falla en el funcionamiento del reloj"
         }


     );
            modelBuilder.Entity<Product>().HasData(
         new Product
         {
             Id = 14,
             Name = "Ari",
             Price = 93600,
             OldPrice = 93600,
             Stock = 1,
             Images = new List<string> { "relojhombre.jpg" },
             Description = "-Analógico\r\n-Sumergible 50 mts\r\n-Caja de acrílico\r\n-Tapa de acero\r\n-Agujas luminiscentes\r\n-Malla de silicona perlada\r\n-Diámetro: 4 cm",
             Color = "Tiza con numeros dorados",
             Brand = "Tressa",
             Specs = "Todos los relojes cuentan con 1 año de garantía oficial de fábrica ante cualquier falla en el funcionamiento del reloj"
         }


     );
            modelBuilder.Entity<Product>().HasData(
         new Product
         {
             Id = 15,
             Name = "Ema",
             Price = 93600,
             OldPrice = 93600,
             Stock = 1,
             Images = new List<string> { "relojhombre.jpg" },
             Description = "-Analógico\r\n-Sumergible 50 mts\r\n-Caja de acrílico\r\n-Tapa de metal\r\n-Malla de silicona\r\n-Diámetro: 3,5cm",
             Color = "Blanco fondo dorado",
             Brand = "Tressa",
             Specs = ""
         }


     );
            modelBuilder.Entity<Product>().HasData(
       new Product
       {
           Id = 16,
           Name = "Naomi",
           Price = 89700,
           OldPrice = 89700,
           Stock = 1,
           Images = new List<string> { "relojhombre.jpg" },
           Description = "– Analógico\r\n– Resistencia al agua\r\n– Calendario\r\n– Strass\r\n– Caja de ABS y aluminio\r\n– Tapa de acero\r\n– Malla de aluminio\r\n– Cierre de acero\r\n– Diámetro del reloj: 42 mm.",
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
