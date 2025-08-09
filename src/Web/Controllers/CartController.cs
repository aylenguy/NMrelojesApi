using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Application.Interfaces;  // o el namespace donde esté definida la interfaz


namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private int GetClientIdFromClaims()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : throw new System.Exception("Cliente no identificado");
        }

        [HttpGet]
        public IActionResult GetCart()
        {
            var clientId = GetClientIdFromClaims();
            var cart = _cartService.GetCartByClientId(clientId);
            return Ok(cart);
        }

        [HttpPost("add")]
        public IActionResult AddItem([FromBody] AddCartItemRequest req)
        {
            var clientId = GetClientIdFromClaims();
            var cart = _cartService.AddItem(clientId, req.ProductId, req.Cantidad);
            return Ok(cart);
        }

        [HttpPut("item/{cartItemId}")]
        public IActionResult UpdateItem(int cartItemId, [FromBody] UpdateCartItemRequest req)
        {
            var clientId = GetClientIdFromClaims();
            var cart = _cartService.UpdateItem(clientId, cartItemId, req.Cantidad);
            return Ok(cart);
        }

        [HttpDelete("item/{cartItemId}")]
        public IActionResult RemoveItem(int cartItemId)
        {
            var clientId = GetClientIdFromClaims();
            var cart = _cartService.RemoveItem(clientId, cartItemId);
            return Ok(cart);
        }

        [HttpPost("clear")]
        public IActionResult ClearCart()
        {
            var clientId = GetClientIdFromClaims();
            _cartService.ClearCart(clientId);
            return Ok();
        }
    }

    public class AddCartItemRequest
    {
        public int ProductId { get; set; }
        public int Cantidad { get; set; }
    }

    public class UpdateCartItemRequest
    {
        public int Cantidad { get; set; }
    }
}
