using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    OldPrice = table.Column<decimal>(type: "TEXT", nullable: true),
                    Images = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Specs = table.Column<string>(type: "TEXT", nullable: false),
                    Color = table.Column<string>(type: "TEXT", nullable: true),
                    Brand = table.Column<string>(type: "TEXT", nullable: true),
                    Stock = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    UserType = table.Column<string>(type: "TEXT", maxLength: 8, nullable: false),
                    UserName = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    LastName = table.Column<string>(type: "TEXT", nullable: true),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    ResetToken = table.Column<string>(type: "TEXT", nullable: true),
                    ResetTokenExpira = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: true),
                    GuestId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Carts_Users_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Ventas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: true),
                    CustomerEmail = table.Column<string>(type: "TEXT", nullable: false),
                    CustomerName = table.Column<string>(type: "TEXT", nullable: false),
                    CustomerLastname = table.Column<string>(type: "TEXT", nullable: false),
                    ExternalReference = table.Column<string>(type: "TEXT", nullable: false),
                    StatusDetail = table.Column<string>(type: "TEXT", nullable: false),
                    PaymentId = table.Column<string>(type: "TEXT", nullable: false),
                    TransactionAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    Street = table.Column<string>(type: "TEXT", nullable: false),
                    Number = table.Column<string>(type: "TEXT", nullable: false),
                    Department = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    City = table.Column<string>(type: "TEXT", nullable: false),
                    Province = table.Column<string>(type: "TEXT", nullable: false),
                    PostalCode = table.Column<string>(type: "TEXT", nullable: false),
                    PaymentStatus = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ShippingMethod = table.Column<string>(type: "TEXT", nullable: false),
                    ShippingAddress = table.Column<string>(type: "TEXT", nullable: false),
                    ShippingCost = table.Column<decimal>(type: "TEXT", nullable: false),
                    PaymentMethod = table.Column<string>(type: "TEXT", nullable: false),
                    DeliveryMethod = table.Column<string>(type: "TEXT", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Total = table.Column<decimal>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ventas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ventas_Users_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CartId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_Carts_CartId",
                        column: x => x.CartId,
                        principalTable: "Carts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetalleVentas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VentaId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    Subtotal = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleVentas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetalleVentas_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetalleVentas_Ventas_VentaId",
                        column: x => x.VentaId,
                        principalTable: "Ventas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Brand", "Color", "Description", "Images", "Name", "OldPrice", "Price", "Specs", "Stock" },
                values: new object[,]
                {
                    { 1, "Kosiuko", "Negro", "Reloj deportivo resistente al agua, ideal para actividades al aire libre.", "[\"relojhombre.jpg\"]", "Reloj Deportivo", 10m, 10m, "Cronómetro, GPS, sumergible hasta 50m", 15 },
                    { 2, "Knock out", "Rose fondo nacarado", "-Analógico\r\n-Resistente a salpicaduras WR30\r\n-Fondo nacarado con números plateados\r\n-Caja de plástico\r\n-Tapa de acero\r\n-Malla extensible de metal\r\n-Agujas luminiscentes\r\n-Diámetro: 3,5 cm", "[\"KnockOutAylen.JPEG\",\"KnockOut2.JPEG\"]", "Aylen (chico)", 81000m, 81000m, "Todos los relojes cuentan con 1 año de garantía oficial de fábrica ante cualquier falla en el funcionamiento del reloj", 1 },
                    { 3, "Kosiuko", "Dorado", "Este reloj tiene malla con eslabones, el cual necesita de un relojero para poder ajustarlo, si queres podes dejarnos en comentarios el tamaño de tu muñeca y nos encargamos de enviártelo ajustado listo para usar.\r\n\r\nOtras características:\r\n-Analógico\r\n-Sumergible WR50\r\n-Agujas luminiscentes\r\n-Calendario\r\n-Caja y malla de acero\r\n-Diámetro: 3,8cm", "[\"KosiukoMica.JPEG\",\"kosiukoMica2.JPEG\"]", "Mica", 110000m, 110000m, "Todos los relojes cuentan con 1 año de garantía oficial de fábrica ante cualquier falla en el funcionamiento del reloj", 1 },
                    { 4, "Kosiuko", "Rose", "Este reloj tiene malla con eslabones...", "[\"relojhombre.jpg\"]", "Flor", 110000m, 110000m, "Todos los relojes cuentan con 1 año de garantía oficial de fábrica ante cualquier falla en el funcionamiento del reloj", 1 },
                    { 5, "Knock out", "Rose fondo nacarado", "-Analógico\r\n-Resistente a salpicaduras: WR30...", "[\"relojhombre.jpg\"]", "Mela", 69330m, 69330m, "Todos los relojes cuentan con 1 año de garantía oficial de fábrica...", 1 },
                    { 6, "Tressa", "rosa bb", "-Analógico\r\n-Sumergible 50 mts...", "[\"relojhombre.jpg\"]", "Margot", 93600m, 93600m, "Todos los relojes cuentan con 1 año de garantía oficial...", 1 },
                    { 7, "Knock out", "Plateado", "Este reloj tiene malla con eslabones...", "[\"relojhombre.jpg\"]", "Mia", 69331m, 69331m, "Todos los relojes cuentan con 1 año de garantía oficial...", 1 },
                    { 8, "Kosiuko", "Plateado con nacar", "Este reloj tiene malla con eslabones...", "[\"relojhombre.jpg\"]", "Abby", 100000m, 100000m, "Todos los relojes cuentan con 1 año de garantía oficial...", 1 },
                    { 9, "Knock out", "Blanco", "-Analógico\r\n-Malla de silicona...", "[\"relojhombre.jpg\"]", "Rita", 64700m, 60480m, "Todos los relojes cuentan con 1 año de garantía oficial...", 1 },
                    { 10, "Knock out", "Plateado", "Este reloj tiene malla con eslabones...", "[\"KosiukoGenova.JPG\",\"KosiukoGenova2.JPG\"]", "Génova", 74050m, 74050m, "Todos los relojes cuentan con 1 año de garantía oficial...", 1 },
                    { 11, "Knock out", "Plateado fondo nacarado", "-Analógico\r\n-Resistente a salpicaduras...", "[\"relojhombre.jpg\"]", "Berlin", 69330m, 69330m, "Todos los relojes cuentan con 1 año de garantía oficial...", 1 },
                    { 12, "Knock out", "Plateado", "-Analógico\r\n-Resistente a salpicaduras...", "[\"relojhombre.jpg\"]", "Paula", 73344m, 73344m, "Todos los relojes cuentan con 1 año de garantía oficial...", 1 },
                    { 13, "Tressa", "Nude", "-Analógico\r\n-Resistente al agua W30...", "[\"relojhombre.jpg\"]", "Lucia", 73920m, 73920m, "Todos los relojes cuentan con 1 año de garantía oficial...", 1 },
                    { 14, "Tressa", "Tiza con numeros dorados", "-Analógico\r\n-Sumergible 50 mts...", "[\"relojhombre.jpg\"]", "Ari", 93600m, 93600m, "Todos los relojes cuentan con 1 año de garantía oficial...", 1 },
                    { 15, "Tressa", "Blanco fondo dorado", "-Analógico\r\n-Sumergible 50 mts...", "[\"relojhombre.jpg\"]", "Ema", 93600m, 93600m, "", 1 },
                    { 16, "Kosiuko", "Negro", "– Analógico\r\n– Resistencia al agua...", "[\"relojhombre.jpg\"]", "Naomi", 89700m, 89700m, "", 1 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "LastName", "Name", "PasswordHash", "ResetToken", "ResetTokenExpira", "UserName", "UserType" },
                values: new object[] { 1, "aylenguy@gmail.com", "Guy", "Aylen", "$2a$11$/Q98DFfyOZlpeJjmBvNITuxkOoV/PKEEFoYJ8nap1O5VLiGsQq3nu", null, null, "aylen", "Admin" });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId",
                table: "CartItems",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductId",
                table: "CartItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_ClientId",
                table: "Carts",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleVentas_ProductId",
                table: "DetalleVentas",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleVentas_VentaId",
                table: "DetalleVentas",
                column: "VentaId");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_ClientId",
                table: "Ventas",
                column: "ClientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "DetalleVentas");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Ventas");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
