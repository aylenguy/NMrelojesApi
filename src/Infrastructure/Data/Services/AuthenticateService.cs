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
        private string GenerateToken(string id, string email, string? username, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, id),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Role, role)
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
            var user = ValidateUser(credentialsRequest);
            if (user == null)
                throw new NotAllowedException("Usuario o contraseña incorrectos");

            var token = GenerateToken(
                user.Id.ToString(),
                user.Email,
                user.UserName,
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
