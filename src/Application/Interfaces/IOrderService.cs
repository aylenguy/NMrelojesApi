using Domain.Entities;
using System.Collections.Generic;

namespace Application.Interfaces
{
    public interface IOrderService
    {
        Venta CreateOrderFromCart(int clientId);
        Venta GetOrderById(int id);
        List<Venta> GetOrdersByClient(int clientId);
    }
}
