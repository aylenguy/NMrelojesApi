using Application.Models.Requests;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IClientService
    {
        int RegisterClient(ClientRegisterRequest request);

        // 🔎 Buscar cliente por email
        Client? GetByEmail(string email);

        // 🔎 Buscar cliente por token de recuperación
        Client? GetByToken(string token);

        // 💾 Actualizar datos del cliente (ej: guardar reset token o nueva password)
        void Update(Client client);
    }
}
