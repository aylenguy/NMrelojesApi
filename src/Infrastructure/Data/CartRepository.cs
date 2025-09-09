using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Infrastructure.Data
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationContext _ctx;

        public CartRepository(ApplicationContext ctx)
        {
            _ctx = ctx;
        }

        // 🔹 Agregar un carrito nuevo
        public Cart Add(Cart cart)
        {
            _ctx.Carts.Add(cart);
            _ctx.SaveChanges();
            return cart;
        }

        // 🔹 Agregar un item a un carrito
        public void AddItem(Cart cart, CartItem item)
        {
            if (cart.Id == 0)
            {
                _ctx.Carts.Add(cart);
                _ctx.SaveChanges();
            }

            item.CartId = cart.Id;
            _ctx.CartItems.Add(item);
            _ctx.SaveChanges();
        }

        // 🔹 Obtener carrito por cliente
        public Cart? GetByClientId(int clientId)
        {
            return _ctx.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefault(c => c.ClientId == clientId);
        }

        // 🔹 Obtener carrito por invitado
        public Cart? GetByGuestId(string guestId)
        {
            return _ctx.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefault(c => c.GuestId == guestId);
        }

        // 🔹 Actualizar carrito (por cambios en cantidades, etc.)
        public void Update(Cart cart)
        {
            _ctx.Carts.Update(cart);
            _ctx.SaveChanges();
        }

        // 🔹 Eliminar un item de cualquier carrito
        public void RemoveItem(Cart cart, CartItem item)
        {
            // Asegurarse que el item pertenece al carrito y está siendo rastreado
            var trackedItem = _ctx.CartItems
                .FirstOrDefault(i => i.Id == item.Id && i.CartId == cart.Id);

            if (trackedItem != null)
            {
                _ctx.CartItems.Remove(trackedItem);
                _ctx.SaveChanges();
            }
        }

        // 🔹 Eliminar un item de un carrito de invitado directamente por guestId y cartItemId
        public void RemoveItemGuest(string guestId, int cartItemId)
        {
            var cart = _ctx.Carts
                .Include(c => c.Items)
                .FirstOrDefault(c => c.GuestId == guestId);

            if (cart == null) throw new Exception("Carrito no encontrado");

            var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);
            if (item == null) throw new Exception("Item no encontrado");

            _ctx.CartItems.Remove(item);
            _ctx.SaveChanges();
        }

        // 🔹 Vaciar carrito completo (cliente o invitado)
        public void ClearCart(Cart cart)
        {
            _ctx.CartItems
                .Where(i => i.CartId == cart.Id)
                .ExecuteDelete(); // hace DELETE directo en la DB sin cargar entidades
        }


    }
}
