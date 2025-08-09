using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using System.Linq;
using System.Collections.Generic;

namespace Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(ICartRepository cartRepository, IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public Venta CreateOrderFromCart(int clientId)
        {
            var cart = _cartRepository.GetByClientId(clientId);
            if (cart == null || cart.Items.Count == 0) throw new System.Exception("Carrito vacío");

            // Validar stock
            foreach (var it in cart.Items)
            {
                var prod = _productRepository.Get(it.ProductId);
                if (prod == null) throw new System.Exception($"Producto {it.ProductId} no encontrado");
                if (prod.Stock < it.Cantidad) throw new System.Exception($"Stock insuficiente para {prod.Name}");
            }

            // Crear Venta
            var venta = new Venta
            {
                ClientId = clientId,
                Fecha = System.DateTime.UtcNow,
                Total = 0m
            };

            venta = _orderRepository.Add(venta);

            foreach (var it in cart.Items)
            {
                var prod = _productRepository.Get(it.ProductId)!;
                var dv = new DetalleVenta
                {
                    VentaId = venta.Id,
                    ProductId = prod.Id,
                    Cantidad = it.Cantidad,
                    PrecioUnitario = prod.Price
                };
                _orderRepository.AddItem(dv);

                // Deduct stock
                prod.Stock -= (int)it.Cantidad; // Convertir a int si Stock es int
                _productRepository.Update(prod);

                venta.Total += dv.Subtotal;
            }

            // Update venta total
            _orderRepository.Update(venta);

            // Limpiar carrito
            _cartRepository.ClearCart(cart);

            return venta;
        }

        public Venta GetOrderById(int id)
        {
            return _orderRepository.GetById(id);
        }

        public List<Venta> GetOrdersByClient(int clientId)
        {
            return _orderRepository.GetByClientId(clientId);
        }
    }
}
