using Application.Interfaces;
using Application.Services;
using Domain.Interfaces;
using Application.Model;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Infrastructure.Data.Services;
using Infrastructure.Repositories;
using Newtonsoft.Json.Serialization; // IMPORTANTE para camelCase
using MercadoPago;
using MercadoPago.Config;

var builder = WebApplication.CreateBuilder(args);

// ===================
// Configuración MercadoPago
// ===================
var mpAccessToken = builder.Configuration["MercadoPago:AccessToken"];
MercadoPagoConfig.AccessToken = mpAccessToken;

// ===================
// Controladores y JSON camelCase
// ===================
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

// ===================
// CORS específico para tu front
// ===================
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .WithOrigins("https://nm-relojes-2yjutova1-aylens-projects-7a096c01.vercel.app") // <--- tu front en Vercel
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ===================
// Swagger con JWT
// ===================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setupAction =>
{
    setupAction.AddSecurityDefinition("EcommerceApiBearerAuth", new OpenApiSecurityScheme()
    {
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        Description = "Pegar el token generado al loguearse."
    });

    setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "EcommerceApiBearerAuth"
                }
            },
            new List<string>()
        }
    });
});

// ===================
// BASE DE DATOS
// ===================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var connection = new SqliteConnection(connectionString);
connection.Open();
using (var command = connection.CreateCommand())
{
    command.CommandText = "PRAGMA journal_mode = DELETE;";
    command.ExecuteNonQuery();
}
connection.Close();

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlite(connectionString));

// ===================
// JWT
// ===================
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<AuthenticateService.JwtOptions>(jwtSettings);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new Exception("JWT Key missing")))
        };
    });

// ===================
// SERVICES & REPOS
// ===================
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IVentaService, VentaService>();
builder.Services.AddScoped<IDetalleVentaService, DetalleVentaService>();
builder.Services.AddScoped<ICustomAuthenticationService, AuthenticateService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IPaymentService, PaymentServiceSandbox>();
builder.Services.AddScoped<IShippingService, ShippingService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IVentaRepository, VentaRepository>();
builder.Services.AddScoped<IDetalleVentaRepository, DetalleVentaRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();

// ===================
// EMAIL
// ===================
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<EmailService>();

var app = builder.Build();

// ===================
// Swagger
// ===================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ===================
// ✅ CORS antes de auth
// ===================
app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

// ===================
// Servir imágenes de /uploads
// ===================
var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
Directory.CreateDirectory(uploadsPath);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

// ===================
// Autenticación y autorización
// ===================
app.UseAuthentication();
app.UseAuthorization();

// ===================
// Mapear controladores
// ===================
app.MapControllers();

// ===================
// Migraciones automáticas
// ===================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    db.Database.Migrate();
}

// ===================
// Configurar puerto dinámico (para Railway/Render)
// ===================
//var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
//app.Urls.Add($"http://*:{port}");

app.Run();
