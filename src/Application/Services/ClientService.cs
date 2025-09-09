using Application.Interfaces;
using Application.Models.Requests;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;

        public ClientService(IUserRepository userRepository, IPasswordService passwordService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
        }

        public int RegisterClient(ClientRegisterRequest request)
        {
            if (_userRepository.GetUserByEmail(request.Email) != null)
                throw new InvalidOperationException("El email ya está registrado");

            var hashedPassword = _passwordService.HashPassword(request.Password);

            var client = new Client
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

        // 🔹 Buscar cliente por email
        public Client? GetByEmail(string email)
        {
            return _userRepository.GetUserByEmail(email) as Client;
        }

        // 🔹 Buscar cliente por token de reseteo
        public Client? GetByToken(string token)
        {
            return _userRepository.GetAll()
                .OfType<Client>()
                .FirstOrDefault(c => c.ResetToken == token);
        }

        // 🔹 Actualizar cliente (guardar cambios)
        public void Update(Client client)
        {
            _userRepository.Update(client);
        }
    }
}
