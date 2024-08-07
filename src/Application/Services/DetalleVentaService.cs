using Application.Interfaces;
using Application.Models;
using Application.Models.Requests;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;


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
            // Validar que la cantidad sea positiva
            if (dto.Amount <= 0)
            {
                throw new NotAllowedException("La cantidad debe ser mayor que cero.");
            }

            // Obtener el producto para verificar su existencia
            var product = _repository.GetProduct(dto.ProductId);
            if (product == null)
            {
                throw new NotAllowedException("El producto no fue encontrado.");
            }

            // Verificar la existencia de la venta
            if (!_repository.VentaExists(dto.VentaId))
            {
                throw new NotAllowedException("La venta no existe.");
            }

            // Verifica stock del producto
            if (product.Stock <= 0)
            {
                throw new NotAllowedException(" Stock insuficiente para el producto.");
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
                UnitPrice = product.Price, 
                Product = product 
            };

            // Actualizar stock del producto
            var updatedProductRequest = new ProductUpdateRequest
            {
                Price = product.Price,
                Stock = product.Stock - dto.Amount
            };
            _productService.UpdateProduct(dto.ProductId, updatedProductRequest);

            //Actualizar el estado del stock
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
                throw new NotAllowedException($"Ningun Detalle de Venta encontrado con el ID: {id}");
            }

            var product = _productService.Get(request.ProductId);
            if (product == null)
            {
                throw new NotAllowedException($"Ningun producto encontrado con el ID: {request.ProductId}");
            }

            // Validar que Amount sea mayor que cero
            if (request.Amount <= 0)
            {
                throw new NotAllowedException("La cantidad debe ser mayor que cero.");
            }

            // Calcular la diferencia de cantidad
            int amountDifference = request.Amount - detalleVentaToUpdate.Amount;

            // Verificar que haya stock
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



