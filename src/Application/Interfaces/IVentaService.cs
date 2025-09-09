using Application.Models;
using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IVentaService
{
    List<VentaResponseDto> GetAll();
    List<VentaResponseDto> GetAllByClient(int clientId);
    VentaResponseDto? GetById(int id);

    int AddVenta(VentaDto dto);
    void UpdateVenta(Venta venta);

    VentaResponseDto UpdateVenta(int id, VentaDto dto);


    void DeleteVenta(int id);

    // Cancela una venta y devuelve el stock
    void CancelVenta(int id);

    
    // Crea una venta a partir del carrito
    Task<VentaResponseDto> CreateFromCart(int clientId, VentaDto dto);


    // Actualiza el estado de la venta
    void UpdateStatus(int id, VentaStatus newStatus);

    List<VentaResponseDto> GetAllByClientOrEmail(int? clientId, string email);

    Task<VentaResponseDto?> GetByExternalReferenceAsync(string externalReference);

    Venta GetEntityById(int id);

    void SetExternalReference(int id, string externalReference);

    Task<Venta> GetEntityByIdAsync(int id, bool includeDetails = false);
}
