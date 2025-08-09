using Application.Interfaces;
using Application.Models.Requests;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Exceptions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services
{
    public class AuthenticateService : ICustomAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly JwtOptions _options;
        private readonly IPasswordService _passwordService;

        public AuthenticateService(
     IUserRepository userRepository,
     IAdminRepository adminRepository,
     IPasswordService passwordService,        // nuevo parámetro
     IOptions<JwtOptions> options)
        {
            _userRepository = userRepository;
            _adminRepository = adminRepository;
            _passwordService = passwordService;       // asignación
            _options = options.Value;
        }
        // Genera el token
        private string GenerateToken(string id, string email, string? username, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
    {
        new Claim("sub", id),
        new Claim("email", email),
        new Claim(ClaimTypes.Role, role) // <-- cambio importante
    };

            if (!string.IsNullOrEmpty(username))
                claims.Add(new Claim("username", username));

            var token = new JwtSecurityToken(
                _options.Issuer,
                _options.Audience,
                claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        // Usuario normal
        public string Authenticate(CredentialsDtoRequest credentialsRequest)
        {
            var user = ValidateUser(credentialsRequest);
            if (user == null)
                throw new NotAllowedException("User authentication failed");

            return GenerateToken(
                user.Id.ToString(),
                user.Email,
                user.UserName,
                user.UserType);
        }

        private User? ValidateUser(CredentialsDtoRequest credentialsRequest)
        {
            if (string.IsNullOrEmpty(credentialsRequest.Email) || string.IsNullOrEmpty(credentialsRequest.Password))
                return null;

            var user = _userRepository.GetUserByEmail(credentialsRequest.Email);
            if (user == null) return null;

            // Usar PasswordService para verificar
            if (!_passwordService.VerifyPassword(credentialsRequest.Password, user.PasswordHash))
                return null;

            return user;
        }

        // Admin
        public string AuthenticateAdmin(CredentialsDtoRequest credentialsRequest)
        {
            if (string.IsNullOrEmpty(credentialsRequest.Email) || string.IsNullOrEmpty(credentialsRequest.Password))
                throw new NotAllowedException("Credenciales inválidas");

            var admin = _adminRepository.GetByEmail(credentialsRequest.Email);
            if (admin == null)
                throw new NotAllowedException("Admin no encontrado");

            // Usar PasswordService para verificar
            if (!_passwordService.VerifyPassword(credentialsRequest.Password, admin.PasswordHash))
                throw new NotAllowedException("Credenciales inválidas");

            return GenerateToken(admin.Id.ToString(), admin.Email, null, "Admin");
        }
        // Configuración para JWT
        public class JwtOptions
        {
            public string Issuer { get; set; } = "";
            public string Audience { get; set; } = "";
            public string Key { get; set; } = "";
        }
    }
}
