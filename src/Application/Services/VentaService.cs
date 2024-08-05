using Application.Interfaces;
using Application.Model;
using Application.Model.Request;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using System.Collections.Generic;

namespace Application.Services
{
    public class VentaService : IVentaService
    {
        private readonly IVentaRepository _ventaRepository;

        public VentaService(IVentaRepository ventaRepository)
        {
            _ventaRepository = ventaRepository;
        }

        public VentaDto GetById(int id)
        {
            var venta = _ventaRepository.GetVentaById(id)
                        ?? throw new NotFoundException(nameof(Venta), id);
            return VentaDto.Create(venta);
        }

        public List<VentaDto> GetAll()
        {
            var list = _ventaRepository.GetAllVentas().ToList();
            return VentaDto.CreateList(list);
        }

        public List<Venta> GetAllFullData()
        {
            return _ventaRepository.GetAllVentas().ToList();
        }

        public Venta Create(VentaCreateRequest ventaCreateRequest)
        {
            var venta = new Venta
            {
                Date = ventaCreateRequest.Date,
                ClientId = ventaCreateRequest.ClientId,
                ProductId = ventaCreateRequest.ProductId,
                Quantity = ventaCreateRequest.Quantity
            };

            return _ventaRepository.AddVenta(venta);
        }

        public void Update(int id, VentaUpdateRequest ventaUpdateRequest)
        {
            var venta = _ventaRepository.GetVentaById(id)
                        ?? throw new NotFoundException(nameof(Venta), id);

            if (ventaUpdateRequest.Date != default) venta.Date = ventaUpdateRequest.Date;
            if (ventaUpdateRequest.ClientId > 0) venta.ClientId = ventaUpdateRequest.ClientId;
            if (ventaUpdateRequest.ProductId > 0) venta.ProductId = ventaUpdateRequest.ProductId;
            if (ventaUpdateRequest.Quantity > 0) venta.Quantity = ventaUpdateRequest.Quantity;

            _ventaRepository.UpdateVenta(venta);
        }

        public void Delete(int id)
        {
            var venta = _ventaRepository.GetVentaById(id)
                        ?? throw new NotFoundException(nameof(Venta), id);
            _ventaRepository.DeleteVenta(id);
        }

        public Venta GetVentaById(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Venta> GetAllVentas()
        {
            throw new NotImplementedException();
        }

        public void AddVenta(Venta venta)
        {
            throw new NotImplementedException();
        }

        public void UpdateVenta(Venta venta)
        {
            throw new NotImplementedException();
        }

        public void DeleteVenta(int id)
        {
            throw new NotImplementedException();
        }
    }
}
