using Application.Interfaces;
using Application.Model;
using Application.Models;
using Application.Models.Requests;
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

        public DetalleVentaService(
            IDetalleVentaRepository detalleVentaRepository,
            IProductRepository productRepository)
        {
            _detalleVentaRepository = detalleVentaRepository;
            _productRepository = productRepository;
        }

        public List<DetalleVenta> GetAllByClient(int clientId)
            => _detalleVentaRepository.GetAllByClient(clientId);

        public List<DetalleVenta> GetAllByProduct(int productId)
            => _detalleVentaRepository.GetAllByProduct(productId);

        public List<DetalleVenta> GetAllByVenta(int ventaId)
            => _detalleVentaRepository.GetAllByVenta(ventaId);

        public DetalleVenta? GetById(int id)
            => _detalleVentaRepository.Get(id);

        public int AddDetalleVenta(DetalleVentaDto dto)
        {
            var producto = _productRepository.GetById(dto.ProductoId)
                ?? throw new Exception("El producto no existe");

            if (producto.Stock < dto.Cantidad)
                throw new Exception("Stock insuficiente para el producto.");

            producto.Stock -= dto.Cantidad;
            _productRepository.Update(producto);

            var detalle = new DetalleVenta
            {
                VentaId = dto.VentaId,
                ProductId = dto.ProductoId,
                Quantity = dto.Cantidad,
                UnitPrice = producto.Price,
                Subtotal = producto.Price * dto.Cantidad
            };

            var saved = _detalleVentaRepository.Add(detalle);
            return saved.Id; // ✅ devolvemos el Id
        }



        public void DeleteDetalleVenta(int id)
        {
            var detalle = _detalleVentaRepository.Get(id)
                ?? throw new Exception("DetalleVenta no encontrado");

            _detalleVentaRepository.Delete(detalle); // ✅ Delete espera la entidad, no Id
        }

        public void UpdateDetalleVenta(int id, DetalleVentaUpdateRequest request)
        {
            var detalle = _detalleVentaRepository.Get(id)
                ?? throw new Exception("Detalle de venta no encontrado");

            var producto = _productRepository.GetById(detalle.ProductId)
                ?? throw new Exception("Producto no encontrado");

            if (producto.Stock + detalle.Quantity < request.Cantidad)
                throw new Exception("Stock insuficiente para actualizar la cantidad.");

            // Reajustar stock (devolvemos lo viejo y descontamos lo nuevo)
            producto.Stock += detalle.Quantity;
            producto.Stock -= request.Cantidad;
            _productRepository.Update(producto);

            detalle.Quantity = request.Cantidad;
            detalle.UnitPrice = producto.Price; // ✅ Reasegurar precio
            detalle.Subtotal = producto.Price * request.Cantidad;

            _detalleVentaRepository.Update(detalle);
        }
    }
}

