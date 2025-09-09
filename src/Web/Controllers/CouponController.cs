using Microsoft.AspNetCore.Mvc;
using Application.Model.Request;
using Application.Model.Response;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CouponController : ControllerBase
    {
        // ⚡ Por ahora hardcodeado, después podés moverlo a base de datos
        private readonly Dictionary<string, decimal> _validCoupons = new()
        {
            { "PROMO10", 0.10m },      // 10% de descuento
            { "BIENVENIDO", 0.15m }    // 15% de descuento
        };

        [HttpPost("apply")]
        public IActionResult ApplyCoupon([FromBody] ApplyCouponRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Code))
                return BadRequest(new { error = "El cupón es obligatorio" });

            if (!_validCoupons.ContainsKey(request.Code.ToUpper()))
                return BadRequest(new { error = "Cupón inválido o no existente" });

            var discountRate = _validCoupons[request.Code.ToUpper()];
            var discount = request.Total * discountRate;

            var response = new ApplyCouponResponse
            {
                Discount = discount,
                NewTotal = request.Total - discount,
                CouponCode = request.Code
            };

            return Ok(response);
        }
    }
}
