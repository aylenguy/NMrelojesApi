using Application.Interfaces;
using Application.Models.Requests;
using Domain.Entities;
using Domain.Interfaces; 
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

public class AuthenticationService : IAuthenticationService
{
    private readonly IConfiguration _config;
    private readonly IUserRepository _userRepository; 
    private readonly AuthenticationServiceOptions _options; 

    public AuthenticationService(IConfiguration config, IUserRepository userRepository, AuthenticationServiceOptions options)
    {
        _config = config;
        _userRepository = userRepository;
        _options = options;
    }

    public User? ValidateUser(AuthenticationRequest authenticationRequest)
    {
        if (string.IsNullOrEmpty(authenticationRequest.Email) || string.IsNullOrEmpty(authenticationRequest.Password))
            return null;

        var user = _userRepository.GetByEmail(authenticationRequest.Email);

        if (user == null) return null;

        if (user.Type == User.UserType.Client || user.Type == User.UserType.Admin)
        {
            if (user.Password == authenticationRequest.Password) return user;
        }

        return null;
    }

    public string Authenticate(AuthenticationRequest authenticationRequest)
    {
        var user = ValidateUser(authenticationRequest);

        if (user == null)
        {
            throw new Exception("User not found");
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretForKey)); 
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Type.ToString() ?? User.UserType.Admin.ToString())
        };

        var jwtToken = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }

    public class AuthenticationServiceOptions
    {
        public const string AuthenticationService = "AuthenticationService";
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretForKey { get; set; }
    }
}
