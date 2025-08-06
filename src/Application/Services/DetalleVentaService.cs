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
        private readonly IVentaRepository _ventaRepository;

        public DetalleVentaService(IDetalleVentaRepository repository, IProductService productService, IVentaRepository ventaRepository)
        {
            _repository = repository;
            _productService = productService;
            _ventaRepository = ventaRepository;
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
            try
            {
                // Validar que la cantidad sea positiva
                if (dto.Amount <= 0)
                {
                    throw new NotAllowedException("La cantidad debe ser mayor que cero.");
                }

                // Obtener el producto para verificar su existencia
                var product = _productService.Get(dto.ProductId);
                if (product == null)
                {
                    throw new NotAllowedException("El producto no fue encontrado.");
                }

                // Verificar la existencia de la venta
                var venta = _ventaRepository.GetById(dto.VentaId);
                if (venta == null)
                {
                    throw new NotAllowedException("La venta no existe.");
                }

                // Verificar stock del producto
                if (product.Stock <= 0)
                {
                    throw new NotAllowedException("Stock insuficiente para el producto.");
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
                    Product = product,
                    Venta = venta
                };

                // Actualizar stock del producto
                var updatedProductRequest = new ProductUpdateRequest
                {
                    Price = product.Price,
                    Stock = product.Stock - dto.Amount
                };
                _productService.UpdateProduct(dto.ProductId, updatedProductRequest);

                return _repository.Add(detalleVenta).Id;
            }
            catch (NotAllowedException)
            {
                // Re-lanzar la excepción original sin encapsularla
                throw;
            }
            catch (Exception ex)
            {
                // Capturar cualquier otra excepción inesperada
                throw new Exception("Ocurrió un error inesperado al agregar detalle de venta.", ex);
            }
        }

        public void DeleteDetalleVenta(int id)
        {
            try
            {
                var detalleVentaToDelete = _repository.Get(id);
                if (detalleVentaToDelete != null)
                {
                    _repository.Delete(detalleVentaToDelete);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al eliminar el detalle de venta.", ex);
            }
        }

        public void UpdateDetalleVenta(int id, DetalleVentaUpdateRequest request)
        {
            try
            {
                var detalleVentaToUpdate = _repository.Get(id);
                if (detalleVentaToUpdate == null)
                {
                    throw new NotAllowedException($"Ningún Detalle de Venta encontrado con el ID: {id}");
                }

                var product = _productService.Get(request.ProductId);
                if (product == null)
                {
                    throw new NotAllowedException($"Ningún producto encontrado con el ID: {request.ProductId}");
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
                detalleVentaToUpdate.Product = product; // Actualizar la propiedad Product
                detalleVentaToUpdate.Venta = _ventaRepository.GetById(detalleVentaToUpdate.VentaId); // Actualizar la propiedad Venta

                _repository.Update(detalleVentaToUpdate);
            }
            catch (NotAllowedException)
            {
                // Re-lanzar la excepción original sin encapsularla
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error inesperado al actualizar detalle de venta.", ex);
            }
        }
    }
}
