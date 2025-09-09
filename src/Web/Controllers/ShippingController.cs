// Controllers/ShippingController.cs
using Application.Interfaces;
using Application.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingController : ControllerBase
    {
        private readonly IShippingService _shippingService;

        public ShippingController(IShippingService shippingService)
        {
            _shippingService = shippingService;
        }

        // ✅ POST -> espera body con { postalCode }
        [HttpPost("calculate")]
        public IActionResult CalculatePost([FromBody] ShippingRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.PostalCode))
                return BadRequest("Debe ingresar un código postal.");

            var options = _shippingService.Calculate(request.PostalCode);
            return Ok(options ?? new List<ShippingOptionDto>());
        }

        // ✅ GET -> espera /api/shipping/calculate/2000
        [HttpGet("calculate/{postalCode}")]
        public IActionResult CalculateGet(string postalCode)
        {
            if (string.IsNullOrWhiteSpace(postalCode))
                return BadRequest("Debe ingresar un código postal.");

            var options = _shippingService.Calculate(postalCode);
            return Ok(options ?? new List<ShippingOptionDto>());
        }
    }

    public class ShippingRequest
    {
        public string PostalCode { get; set; }
    }
}
