using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

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

        // 🔹 Mapeo Entidad -> DTO
        private CartDto MapToDto(Cart cart)
        {
            return new CartDto
            {
                Id = cart.Id,
                ClientId = cart.ClientId,
                GuestId = cart.GuestId,
                Items = cart.Items?.Select(i => new CartItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    Brand = i.Product.Brand,
                    Quantity = i.Quantity,
                    UnitPrice = i.Product.Price,
                    ImageUrl = i.Product.Image
                }).ToList() ?? new List<CartItemDto>()
            };
        }

        // ------------------- CLIENTE -------------------

        public CartDto CreateCartForClient(int clientId)
        {
            var existing = _cartRepository.GetByClientId(clientId);
            if (existing != null) return MapToDto(existing);

            var newCart = new Cart { ClientId = clientId, Items = new List<CartItem>() };
            _cartRepository.Add(newCart);
            return MapToDto(newCart);
        }

        public CartDto AddItem(int clientId, int productId, int cantidad)
        {
            var cart = _cartRepository.GetByClientId(clientId)
                       ?? _cartRepository.Add(new Cart { ClientId = clientId, Items = new List<CartItem>() });

            var product = _productRepository.GetById(productId);
            if (product == null) throw new Exception("Producto no encontrado");

            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            int nuevaCantidad = (item?.Quantity ?? 0) + cantidad;

            if (nuevaCantidad > product.Stock)
                throw new Exception($"No hay stock suficiente. Stock disponible: {product.Stock}");

            if (item != null)
            {
                item.Quantity = nuevaCantidad;
                _cartRepository.Update(cart);
            }
            else
            {
                var newItem = new CartItem
                {
                    ProductId = productId,
                    Quantity = cantidad,
                    Product = product // ✅ aseguramos que el producto quede seteado
                };
                _cartRepository.AddItem(cart, newItem);
            }

            // 🔹 Volvemos a cargar el carrito desde el repo para evitar datos viejos
            var updatedCart = _cartRepository.GetByClientId(clientId);
            return MapToDto(updatedCart);
        }

        public CartDto UpdateItem(int clientId, int cartItemId, int cantidad)
        {
            var cart = _cartRepository.GetByClientId(clientId);
            if (cart == null) throw new Exception("Carrito no encontrado");

            var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);
            if (item == null) throw new Exception("Item no encontrado");

            var product = _productRepository.GetById(item.ProductId);
            if (product == null) throw new Exception("Producto no encontrado");

            if (cantidad > product.Stock)
                throw new Exception($"No hay stock suficiente. Stock disponible: {product.Stock}");

            item.Quantity = cantidad;
            _cartRepository.Update(cart);

            // 🔹 Volvemos a cargar el carrito actualizado
            var updatedCart = _cartRepository.GetByClientId(clientId);
            return MapToDto(updatedCart);
        }

        public CartDto RemoveItem(int clientId, int cartItemId)
        {
            var cart = _cartRepository.GetByClientId(clientId);
            if (cart == null) throw new Exception("Carrito no encontrado");

            var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);
            if (item == null) throw new Exception("Item no encontrado");

            _cartRepository.RemoveItem(cart, item);

            // 🔹 Recargamos el carrito limpio
            var updatedCart = _cartRepository.GetByClientId(clientId);
            return MapToDto(updatedCart);
        }


        public CartDto GetCartByClientId(int clientId)
        {
            var cart = _cartRepository.GetByClientId(clientId);
            return cart is null ? null : MapToDto(cart);
        }

        public void ClearCart(int clientId)
        {
            var cart = _cartRepository.GetByClientId(clientId);
            if (cart == null) return;

            _cartRepository.ClearCart(cart);
        }

        // ------------------- INVITADO -------------------

        public CartDto GetCartByGuestId(string guestId)
        {
            var cart = _cartRepository.GetByGuestId(guestId)
                       ?? new Cart { GuestId = guestId, Items = new List<CartItem>() };

            return MapToDto(cart);
        }

        public CartDto AddItemGuest(string guestId, int productId, int cantidad)
        {
            var cart = _cartRepository.GetByGuestId(guestId)
                       ?? _cartRepository.Add(new Cart { GuestId = guestId, Items = new List<CartItem>() });

            var product = _productRepository.GetById(productId);
            if (product == null) throw new Exception("Producto no encontrado");

            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            int nuevaCantidad = (item?.Quantity ?? 0) + cantidad;

            if (nuevaCantidad > product.Stock)
                throw new Exception($"No hay stock suficiente. Stock disponible: {product.Stock}");

            if (item != null)
            {
                item.Quantity = nuevaCantidad;
                _cartRepository.Update(cart);
            }
            else
            {
                var newItem = new CartItem
                {
                    ProductId = productId,
                    Quantity = cantidad,
                    Product = product // ✅ siempre asignamos el producto
                };
                _cartRepository.AddItem(cart, newItem);
            }

            // 🔹 Recargamos el carrito actualizado
            var updatedCart = _cartRepository.GetByGuestId(guestId);
            return MapToDto(updatedCart);
        }

        public CartDto UpdateItemGuest(string guestId, int cartItemId, int cantidad)
        {
            var cart = _cartRepository.GetByGuestId(guestId);
            if (cart == null) throw new Exception("Carrito no encontrado");

            var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);
            if (item == null) throw new Exception("Item no encontrado");

            var product = _productRepository.GetById(item.ProductId);
            if (product == null) throw new Exception("Producto no encontrado");

            if (cantidad > product.Stock)
                throw new Exception($"No hay stock suficiente. Stock disponible: {product.Stock}");

            item.Quantity = cantidad;
            _cartRepository.Update(cart);

            // 🔹 Recargamos el carrito actualizado
            var updatedCart = _cartRepository.GetByGuestId(guestId);
            return MapToDto(updatedCart);
        }

        public CartDto RemoveItemGuest(string guestId, int cartItemId)
        {
            var cart = _cartRepository.GetByGuestId(guestId);
            if (cart == null) throw new Exception("Carrito no encontrado");

            var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);
            if (item == null) throw new Exception("Item no encontrado");

            _cartRepository.RemoveItem(cart, item);

            // 🔹 Recargamos el carrito limpio
            var updatedCart = _cartRepository.GetByGuestId(guestId);
            return MapToDto(updatedCart);
        }

        public void ClearCartGuest(string guestId)
        {
            var cart = _cartRepository.GetByGuestId(guestId);
            if (cart == null) return;

            _cartRepository.ClearCart(cart);
        }
    }
}
