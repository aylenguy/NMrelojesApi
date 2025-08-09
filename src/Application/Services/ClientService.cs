using Application.Interfaces;
using Application.Models.Requests;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;  // <-- inyectar

        public ClientService(IUserRepository userRepository, IPasswordService passwordService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;  // <-- asignar
        }

        public int RegisterClient(ClientRegisterRequest request)
        {
            if (_userRepository.GetUserByEmail(request.Email) != null)
                throw new Exception("El email ya está registrado");

            // Usar PasswordService para hashear la contraseña
            var hashedPassword = _passwordService.HashPassword(request.Password);

            var client = new User
            {
                Email = request.Email,
                UserName = request.UserName,
                Name = request.Name,
                LastName = request.LastName,
                PasswordHash = hashedPassword,
                UserType = "Client"
            };

            _userRepository.Add(client);
            return client.Id;
        }
    }
}
