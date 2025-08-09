using Domain.Entities;

namespace Application.Interfaces
{
    public interface ICartService
    {
        Cart GetCartByClientId(int clientId);
        Cart CreateCartForClient(int clientId);
        Cart AddItem(int clientId, int productId, int cantidad);
        Cart UpdateItem(int clientId, int cartItemId, int cantidad);
        Cart RemoveItem(int clientId, int cartItemId);
        void ClearCart(int clientId);
    }
}
