using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private int GetClientIdFromClaims()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : throw new System.Exception("Cliente no identificado");
        }

        [HttpPost("create")]
        public IActionResult CreateOrder()
        {
            var clientId = GetClientIdFromClaims();
            var venta = _orderService.CreateOrderFromCart(clientId);
            return Ok(venta);
        }

        [HttpGet("my-orders")]
        public IActionResult MyOrders()
        {
            var clientId = GetClientIdFromClaims();
            var orders = _orderService.GetOrdersByClient(clientId);
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public IActionResult GetOrder(int id)
        {
            var order = _orderService.GetOrderById(id);
            if (order == null) return NotFound();
            return Ok(order);
        }
    }
}
