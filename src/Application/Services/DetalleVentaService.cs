using Application.Interfaces;
using Application.Models;
using Application.Models.Requests;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class DetalleVentaService : IDetalleVentaService
    {
        private readonly IDetalleVentaRepository _repository;
        private readonly IProductService _productService;

        public DetalleVentaService(IDetalleVentaRepository repository, IProductService productService)
        {
            _repository = repository;
            _productService = productService;
        }

        public List<DetalleVenta> GetAllByClient(int clientId)
        {
            return _repository.GetAllByClient(clientId);
        }

        public List<DetalleVenta> GetAllByProduct(int productId)
        {
            return _repository.GetAllByProduct(productId);
        }

        public List<DetalleVenta> GetAllByVenta(int ventaId)
        {
            return _repository.GetAllByVenta(ventaId);
        }

        public DetalleVenta? GetById(int id)
        {
            return _repository.GetById(id);
        }

        public int AddDetalleVenta(DetalleVentaDto dto)
        {
            // Validar que Amount sea mayor que cero
            if (dto.Amount <= 0)
            {
                throw new NotAllowedException("La cantidad debe ser mayor que cero.");
            }

            // Obtén el producto para asegurarte de que no sea nulo
            var product = _repository.GetProduct(dto.ProductId);
            if (product == null)
            {
                throw new NotAllowedException("El producto no se pudo encontrar.");
            }

            // Verifica que el VentaId exista en la tabla de órdenes de venta
            if (!_repository.VentaExists(dto.VentaId))
            {
                throw new NotAllowedException("VentaId no existe.");
            }

            // Verifica el stock del producto
            if (product.Stock <= 0)
            {
                throw new NotAllowedException("El producto no está disponible en stock.");
            }

            if (product.Stock < dto.Amount)
            {
                throw new NotAllowedException("No hay suficiente stock para el producto.");
            }

            var detalleVenta = new DetalleVenta()
            {
                ProductId = dto.ProductId,
                VentaId = dto.VentaId,
                Amount = dto.Amount,
                UnitPrice = product.Price, // Asigna el precio unitario del producto
                Product = product // Asigna el producto para evitar referencias nulas
            };

            // Actualiza el stock del producto
            var updatedProductRequest = new ProductUpdateRequest
            {
                Price = product.Price,
                Stock = product.Stock - dto.Amount
            };
            _productService.UpdateProduct(dto.ProductId, updatedProductRequest);

            // Verificar y actualizar el estado del stock
            product.Stock = updatedProductRequest.Stock;

            return _repository.Add(detalleVenta).Id;
        }

        public void DeleteDetalleVenta(int id)
        {
            var detalleVentaToDelete = _repository.Get(id);
            if (detalleVentaToDelete != null)
            {
                _repository.Delete(detalleVentaToDelete);
            }
        }

        public void UpdateDetalleVenta(int id, DetalleVentaUpdateRequest request)
        {
            var detalleVentaToUpdate = _repository.Get(id);
            if (detalleVentaToUpdate == null)
            {
                throw new NotAllowedException($"No se encontró ningun Detalle de Venta con el ID: {id}");
            }

            var product = _productService.Get(request.ProductId);
            if (product == null)
            {
                throw new NotAllowedException($"No se encontró ningun Producto con el ID: {request.ProductId}");
            }

            // Validar que Amount sea mayor que cero
            if (request.Amount <= 0)
            {
                throw new NotAllowedException("La cantidad debe ser mayor que cero.");
            }

            // Calcular la diferencia de cantidad
            int amountDifference = request.Amount - detalleVentaToUpdate.Amount;

            // Verificar que haya suficiente stock
            if (product.Stock < amountDifference)
            {
                throw new NotAllowedException("No hay suficiente stock para el producto.");
            }

            // Actualizar el stock del producto
            var updatedProductRequest = new ProductUpdateRequest
            {
                Price = product.Price,
                Stock = product.Stock - amountDifference
            };
            _productService.UpdateProduct(request.ProductId, updatedProductRequest);

            // Actualizar el detalle de la orden de venta
            detalleVentaToUpdate.Amount = request.Amount;
            detalleVentaToUpdate.ProductId = request.ProductId;

            _repository.Update(detalleVentaToUpdate);
        }
    }
}



