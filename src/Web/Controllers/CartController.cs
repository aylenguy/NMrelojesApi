using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Por defecto, requiere auth
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // 🔹 Obtiene ClientId desde claims
        private int GetClientIdFromClaims()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (claim == null) throw new Exception("Cliente no identificado");
            return int.Parse(claim.Value);
        }

        // =============================
        // ====== CLIENTE LOGUEADO =====
        // =============================

        [HttpGet]
        public IActionResult GetCart()
        {
            var clientId = GetClientIdFromClaims();
            var cart = _cartService.GetCartByClientId(clientId);

            // 🔹 Si el carrito es null, devolver uno vacío
            if (cart == null)
            {
                cart = new CartDto
                {
                    Items = new List<CartItemDto>() // Total se calcula automáticamente
                };
            }

            // 🔹 Agregar URL completa a cada imagen
            if (cart.Items != null)
            {
                foreach (var item in cart.Items)
                {
                    if (item.Images != null && item.Images.Any())
                    {
                        item.Images = item.Images
                            .Select(img => $"{Request.Scheme}://{Request.Host}/uploads/{img}")
                            .ToList();
                    }
                }
            }

            return Ok(cart);
        }

        [HttpPost("add")]
        public IActionResult AddItem([FromBody] AddCartItemRequest req)
        {
            try
            {
                var clientId = GetClientIdFromClaims();
                var cart = _cartService.AddItem(clientId, req.ProductId, req.Quantity);

                foreach (var item in cart.Items)
                {
                    if (item.Images != null && item.Images.Any())
                    {
                        item.Images = item.Images
                            .Select(img => $"{Request.Scheme}://{Request.Host}/uploads/{img}")
                            .ToList();
                    }
                }

                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("item/{cartItemId}")]
        public IActionResult UpdateItem(int cartItemId, [FromBody] UpdateCartItemRequest req)
        {
            try
            {
                var clientId = GetClientIdFromClaims();
                var cart = _cartService.UpdateItem(clientId, cartItemId, req.Quantity);

                foreach (var item in cart.Items)
                {
                    if (item.Images != null && item.Images.Any())
                    {
                        item.Images = item.Images
                            .Select(img => $"{Request.Scheme}://{Request.Host}/uploads/{img}")
                            .ToList();
                    }
                }

                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("item/{cartItemId}")]
        public IActionResult RemoveItem(int cartItemId)
        {
            var clientId = GetClientIdFromClaims();
            var cart = _cartService.RemoveItem(clientId, cartItemId);

            foreach (var item in cart.Items)
            {
                if (item.Images != null && item.Images.Any())
                {
                    item.Images = item.Images
                        .Select(img => $"{Request.Scheme}://{Request.Host}/uploads/{img}")
                        .ToList();
                }
            }

            return Ok(cart);
        }

        [HttpPost("clear")]
        public IActionResult Clear()
        {
            var clientId = GetClientIdFromClaims();
            _cartService.ClearCart(clientId);
            return Ok();
        }

        // =============================
        // ======== INVITADO ===========
        // =============================

        [AllowAnonymous]
        [HttpGet("guest")]
        public IActionResult GetCartGuest([FromQuery] string guestId)
        {
            if (string.IsNullOrEmpty(guestId))
                return BadRequest("GuestId es requerido");

            var cart = _cartService.GetCartByGuestId(guestId);

            if (cart == null)
                return NotFound("Carrito no encontrado para este guestId");

            foreach (var item in cart.Items ?? new List<CartItemDto>())
            {
                if (item.Images != null && item.Images.Any())
                {
                    item.Images = item.Images
                        .Select(img => $"{Request.Scheme}://{Request.Host}/uploads/{img}")
                        .ToList();
                }
            }

            return Ok(cart);
        }

        [AllowAnonymous]
        [HttpPost("guest/add")]
        public IActionResult AddItemGuest([FromQuery] string guestId, [FromBody] AddCartItemRequest req)
        {
            try
            {
                if (string.IsNullOrEmpty(guestId))
                    guestId = Guid.NewGuid().ToString();

                var cart = _cartService.AddItemGuest(guestId, req.ProductId, req.Quantity);

                foreach (var item in cart.Items)
                {
                    if (item.Images != null && item.Images.Any())
                    {
                        item.Images = item.Images
                            .Select(img => $"{Request.Scheme}://{Request.Host}/uploads/{img}")
                            .ToList();
                    }
                }

                return Ok(new { guestId, cart });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPut("guest/item/{cartItemId}")]
        public IActionResult UpdateItemGuest(string guestId, int cartItemId, [FromBody] UpdateCartItemRequest req)
        {
            try
            {
                if (string.IsNullOrEmpty(guestId))
                    return BadRequest("GuestId es requerido");

                var cart = _cartService.UpdateItemGuest(guestId, cartItemId, req.Quantity);

                foreach (var item in cart.Items)
                {
                    if (item.Images != null && item.Images.Any())
                    {
                        item.Images = item.Images
                            .Select(img => $"{Request.Scheme}://{Request.Host}/uploads/{img}")
                            .ToList();
                    }
                }

                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpDelete("guest/item/{cartItemId}")]
        public IActionResult RemoveItemGuest(string guestId, int cartItemId)
        {
            if (string.IsNullOrEmpty(guestId))
                return BadRequest("GuestId es requerido");

            var cart = _cartService.RemoveItemGuest(guestId, cartItemId);

            foreach (var item in cart.Items)
            {
                if (item.Images != null && item.Images.Any())
                {
                    item.Images = item.Images
                        .Select(img => $"{Request.Scheme}://{Request.Host}/uploads/{img}")
                        .ToList();
                }
            }

            return Ok(cart);
        }

        [AllowAnonymous]
        [HttpPost("guest/clear")]
        public IActionResult ClearGuest([FromQuery] string guestId)
        {
            if (string.IsNullOrEmpty(guestId))
                return BadRequest("GuestId es requerido");

            _cartService.ClearCartGuest(guestId);
            return Ok();
        }
    }

    // ====== REQUEST MODELS ======
    public class AddCartItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateCartItemRequest
    {
        public int Quantity { get; set; }
    }
}
