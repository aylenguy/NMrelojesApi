using Application.Interfaces;
using Application.Models.Requests;
using Application.Model;

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
            IPasswordService passwordService,
            IOptions<JwtOptions> options)
        {
            _userRepository = userRepository;
            _adminRepository = adminRepository;
            _passwordService = passwordService;
            _options = options.Value;
        }

        // 🔹 Generar token JWT con rol y datos opcionales
        private string GenerateToken(string id, string email, string? username, string name, string lastName, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, id),
        new Claim(JwtRegisteredClaimNames.Email, email),
        new Claim(ClaimTypes.Role, role),
        new Claim("name", name),
        new Claim("lastName", lastName)
    };

            if (!string.IsNullOrEmpty(username))
                claims.Add(new Claim("username", username));

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        // 🔹 Login Cliente
        public AuthResult Authenticate(CredentialsDtoRequest credentialsRequest)
        {
            // 1. Validar email vacío
            if (string.IsNullOrEmpty(credentialsRequest.Email))
            {
                return new AuthResult { Error = "email_required" };
            }

            // 2. Validar formato de email
            if (!System.Text.RegularExpressions.Regex.IsMatch(
                    credentialsRequest.Email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return new AuthResult { Error = "invalid_email" };
            }

            // 3. Validar contraseña vacía
            if (string.IsNullOrEmpty(credentialsRequest.Password))
            {
                return new AuthResult { Error = "password_required" };
            }

            // 4. Buscar usuario
            var user = _userRepository.GetUserByEmail(credentialsRequest.Email);
            if (user == null)
            {
                return new AuthResult { Error = "user_not_found" };
            }

            // 5. Verificar contraseña
            if (!_passwordService.VerifyPassword(credentialsRequest.Password, user.PasswordHash))
            {
                return new AuthResult { Error = "wrong_password" };
            }

            // 6. Todo bien → generar token
            var token = GenerateToken(
     user.Id.ToString(),
     user.Email,
     user.UserName,
     user.Name,
     user.LastName,
     "Client"
 );

            return new AuthResult
            {
                Token = token,
                UserType = "Client"
            };
        }



        private User? ValidateUser(CredentialsDtoRequest credentialsRequest)
        {
            if (string.IsNullOrEmpty(credentialsRequest.Email) || string.IsNullOrEmpty(credentialsRequest.Password))
                return null;

            var user = _userRepository.GetUserByEmail(credentialsRequest.Email);
            if (user == null) return null;

            if (!_passwordService.VerifyPassword(credentialsRequest.Password, user.PasswordHash))
                return null;

            return user;
        }

        // 🔹 Login Admin
        public AuthResult AuthenticateAdmin(CredentialsDtoRequest credentialsRequest)
        {
            if (string.IsNullOrEmpty(credentialsRequest.Email) || string.IsNullOrEmpty(credentialsRequest.Password))
                throw new NotAllowedException("Credenciales inválidas");

            var admin = _adminRepository.GetByEmail(credentialsRequest.Email);
            if (admin == null)
                throw new NotAllowedException("Admin no encontrado");

            if (!_passwordService.VerifyPassword(credentialsRequest.Password, admin.PasswordHash))
                throw new NotAllowedException("Credenciales inválidas");

            var token = GenerateToken(
     admin.Id.ToString(),
     admin.Email,
     admin.UserName,
     admin.UserName, // lo usamos como "name"
     "",             // sin apellido
     "Admin"
 );

            return new AuthResult
            {
                Token = token,
                UserType = "Admin"
            };
        }

        // 🔹 Configuración JWT
        public class JwtOptions
        {
            public string Issuer { get; set; } = "";
            public string Audience { get; set; } = "";
            public string Key { get; set; } = "";
        }
    }
}
