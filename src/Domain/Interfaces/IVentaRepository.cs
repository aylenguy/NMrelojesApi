// Domain/Interfaces/IVentaRepository.cs
using Domain.Entities;
using Domain.Interfaces;

public interface IVentaRepository : IRepositoryBase<Venta>
{
    List<Venta> GetAll();
    List<Venta> GetAllByClient(int clientId);

    // 🔹 Nuevo método para buscar por ExternalReference
    Task<Venta?> GetByExternalReferenceAsync(string externalReference);
    Task UpdateAsync(Venta venta);

   // Task AddAsync(Venta venta);   // 👈 agregar esto
    Venta GetById(int id);

    Task<Venta> GetByIdWithDetailsAsync(int id);




}
