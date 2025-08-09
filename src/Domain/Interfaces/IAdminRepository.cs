using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IAdminRepository : IRepositoryBase<Admin>
    {
        Admin? Get(string name);
        Admin? GetByEmail(string email); // 👈 Este es el nuevo método
    }
}
