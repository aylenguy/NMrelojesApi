using Application.DTOs;

namespace Application.Interfaces
{
    public interface ICartService
    {
        // 🔹 Cliente registrado
        CartDto GetCartByClientId(int clientId);
        CartDto CreateCartForClient(int clientId);
        CartDto AddItem(int clientId, int productId, int cantidad);
        CartDto UpdateItem(int clientId, int cartItemId, int cantidad);
        CartDto RemoveItem(int clientId, int cartItemId);
        void ClearCart(int clientId);

        // 🔹 Invitado (guest)
        CartDto GetCartByGuestId(string guestId);
        CartDto AddItemGuest(string guestId, int productId, int cantidad);
        CartDto UpdateItemGuest(string guestId, int cartItemId, int cantidad);
        CartDto RemoveItemGuest(string guestId, int cartItemId);
        void ClearCartGuest(string guestId);
    }
}
