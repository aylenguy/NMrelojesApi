using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IClientRepository
    {
        User? GetClientByEmail(string email);
        void Add(User client);

        void Update(User client); // 🔹 Nuevo método
    }
}
