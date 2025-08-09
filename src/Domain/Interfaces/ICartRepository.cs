using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ICartRepository
    {
        Cart? GetByClientId(int clientId);
        Cart Add(Cart cart);
        void Update(Cart cart);
        void AddItem(Cart cart, CartItem item);
        void RemoveItem(Cart cart, CartItem item);
        void ClearCart(Cart cart);
    }
}
