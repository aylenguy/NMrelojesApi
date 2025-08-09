using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;

namespace Application.Services
{
    public class VentaService : IVentaService
    {
        private readonly IVentaRepository _ventaRepository;
        private readonly IProductRepository _productRepository;

        public VentaService(IVentaRepository ventaRepository, IProductRepository productRepository)
        {
            _ventaRepository = ventaRepository;
            _productRepository = productRepository;
        }

        public List<Venta> GetAllByClient(int clientId)
            => _ventaRepository.GetAllByClient(clientId);

        public Venta? GetById(int id)
            => _ventaRepository.GetById(id);

        public int AddVenta(VentaDto dto)
        {
            var venta = new Venta
            {
                ClientId = dto.ClientId,
                Fecha = DateTime.Now,
                DetalleVentas = new List<DetalleVenta>()
            };

            foreach (var item in dto.Items)
            {
                var product = _productRepository.Get(item.ProductId)
                    ?? throw new Exception($"Producto con ID {item.ProductId} no encontrado.");

                if (product.Stock < item.Cantidad)
                    throw new Exception($"Stock insuficiente para el producto {product.Name}.");

                // Descontar stock
                product.Stock -= item.Cantidad;
                _productRepository.Update(product);

                // Agregar detalle de venta
                venta.DetalleVentas.Add(new DetalleVenta
                {
                    ProductId = item.ProductId,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = product.Price
                });
            }

            // Guardar la venta y obtener el ID generado
            _ventaRepository.Add(venta);
            return venta.Id; // Ahora ya tiene el valor correcto

        }

        public void DeleteVenta(int id)
        {
            var venta = _ventaRepository.GetById(id);
            if (venta != null)
            {
                _ventaRepository.Delete(venta);
            }
        }

        public void UpdateVenta(int id, VentaDto dto)
        {
            var venta = _ventaRepository.GetById(id)
                ?? throw new Exception($"Venta con ID {id} no encontrada.");

            venta.ClientId = dto.ClientId;
            _ventaRepository.Update(venta);
        }
    }
}
