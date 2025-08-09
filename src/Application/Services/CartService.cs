using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using System.Linq;

namespace Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public Cart CreateCartForClient(int clientId)
        {
            var existing = _cartRepository.GetByClientId(clientId);
            if (existing != null) return existing;

            var cart = new Cart { ClientId = clientId };
            return _cartRepository.Add(cart);
        }

        public Cart GetCartByClientId(int clientId)
        {
            return _cartRepository.GetByClientId(clientId) ?? CreateCartForClient(clientId);
        }

        public Cart AddItem(int clientId, int productId, int cantidad)
        {
            var cart = GetCartByClientId(clientId);
            var product = _productRepository.Get(productId);
            if (product == null) throw new System.Exception("Producto no encontrado");
            if (cantidad <= 0) throw new System.Exception("Cantidad inválida");

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Cantidad += cantidad;
                _cartRepository.Update(cart);
                return cart;
            }

            var item = new CartItem
            {
                ProductId = productId,
                Cantidad = cantidad,
                CartId = cart.Id
            };

            _cartRepository.AddItem(cart, item);
            return _cartRepository.GetByClientId(clientId)!;
        }

        public Cart UpdateItem(int clientId, int cartItemId, int cantidad)
        {
            var cart = GetCartByClientId(clientId);
            var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);
            if (item == null) throw new System.Exception("Item no encontrado");
            if (cantidad <= 0) throw new System.Exception("Cantidad inválida");

            item.Cantidad = cantidad;
            _cartRepository.Update(cart);
            return _cartRepository.GetByClientId(clientId)!;
        }

        public Cart RemoveItem(int clientId, int cartItemId)
        {
            var cart = GetCartByClientId(clientId);
            var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);
            if (item == null) throw new System.Exception("Item no encontrado");

            _cartRepository.RemoveItem(cart, item);
            return _cartRepository.GetByClientId(clientId)!;
        }

        public void ClearCart(int clientId)
        {
            var cart = GetCartByClientId(clientId);
            _cartRepository.ClearCart(cart);
        }
    }
}

