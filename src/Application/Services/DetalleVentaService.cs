using Application.Interfaces;
using Application.Models; // Para DetalleVentaDto
using Application.Models.Requests; // Para DetalleVentaUpdateRequest
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;

namespace Application.Services
{
    public class DetalleVentaService : IDetalleVentaService
    {
        private readonly IDetalleVentaRepository _detalleVentaRepository;
        private readonly IProductRepository _productRepository;
        private readonly IVentaRepository _ventaRepository;

        public DetalleVentaService(
            IDetalleVentaRepository detalleVentaRepository,
            IProductRepository productRepository,
            IVentaRepository ventaRepository)
        {
            _detalleVentaRepository = detalleVentaRepository;
            _productRepository = productRepository;
            _ventaRepository = ventaRepository;
        }

        public List<DetalleVenta> GetAllByClient(int clientId)
            => _detalleVentaRepository.GetAllByClient(clientId);

        public List<DetalleVenta> GetAllByProduct(int productId)
            => _detalleVentaRepository.GetAllByProduct(productId);

        public List<DetalleVenta> GetAllByVenta(int ventaId)
            => _detalleVentaRepository.GetAllByVenta(ventaId);

        public DetalleVenta? GetById(int id)
            => _detalleVentaRepository.GetById(id);

        public int AddDetalleVenta(DetalleVentaDto dto)
        {
            var venta = _ventaRepository.GetById(dto.VentaId)
                ?? throw new Exception("La venta no existe");

            var producto = _productRepository.Get(dto.ProductId)
                ?? throw new Exception("El producto no existe");

            if (producto.Stock < dto.Cantidad)
                throw new Exception("Stock insuficiente para el producto.");

            producto.Stock -= dto.Cantidad;
            _productRepository.Update(producto);

            var detalle = new DetalleVenta
            {
                VentaId = dto.VentaId,
                ProductId = dto.ProductId,
                Cantidad = dto.Cantidad,
                PrecioUnitario = producto.Price
            };

            return _detalleVentaRepository.Add(detalle);
        }

        public void DeleteDetalleVenta(int id)
            => _detalleVentaRepository.Delete(id);

        public void UpdateDetalleVenta(int id, DetalleVentaUpdateRequest request)
        {
            var detalle = _detalleVentaRepository.GetById(id)
                ?? throw new Exception("Detalle de venta no encontrado");

            detalle.Cantidad = request.Amount;
            _detalleVentaRepository.Update(detalle);
        }
    }
}
