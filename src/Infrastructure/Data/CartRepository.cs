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

        public Cart Add(Cart cart)
        {
            _ctx.Carts.Add(cart);
            _ctx.SaveChanges();
            return cart;
        }

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

        public Cart? GetByClientId(int clientId)
        {
            return _ctx.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefault(c => c.ClientId == clientId);
        }

        public void RemoveItem(Cart cart, CartItem item)
        {
            _ctx.CartItems.Remove(item);
            _ctx.SaveChanges();
        }

        public void Update(Cart cart)
        {
            _ctx.Carts.Update(cart);
            _ctx.SaveChanges();
        }

        public void ClearCart(Cart cart)
        {
            var items = _ctx.CartItems.Where(i => i.CartId == cart.Id).ToList();
            _ctx.CartItems.RemoveRange(items);
            _ctx.SaveChanges();
        }
    }
}
