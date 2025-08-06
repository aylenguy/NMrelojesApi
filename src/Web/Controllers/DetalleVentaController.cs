using Application.Interfaces;
using Application.Models;
using Application.Models.Requests;
using Application.Services;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class DetalleVentaController : ControllerBase
    {
        private readonly IDetalleVentaService _detalleVentaService;
        private readonly IVentaService _ventaService;
        private readonly IProductService _productService;

        public DetalleVentaController(IDetalleVentaService detalleVentaService, IVentaService ventaService, IProductService productService)
        {
            _detalleVentaService = detalleVentaService;
            _ventaService = ventaService;
            _productService = productService;
        }

        private bool IsUserInRole(string role)
        {
            var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role); 
            return roleClaim != null && roleClaim.Value == role; //Verificar si el claim existe y su valor es "role"
        }

        private int? GetUserId() 
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }
            return null;
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Forbid();
            }

            var detalleVenta = _detalleVentaService.GetById(id);
            if (detalleVenta == null)
            {
                return NotFound($"Ningún detalle de venta encontrado con el ID: {id} ");
            }

            var venta = _ventaService.GetById(detalleVenta.VentaId);
            if (venta == null)
            {
                return NotFound($"Ninguna venta encontrada con el ID: {detalleVenta.VentaId}");
            }

            var isAdmin = IsUserInRole("Admin");
            var isClient = IsUserInRole("Client");

            if (isAdmin || (isClient && userId == venta.ClientId))
            {
                return Ok(detalleVenta);
            }

            return Forbid();
        }


        [HttpGet("{productId}")]
        public IActionResult GetAllByProduct([FromRoute] int productId)
        {
            if (IsUserInRole("Admin"))
            {
                var product = _productService.Get(productId);
                if (product == null)
                {
                    return NotFound($" Ningún producto encontrado con el ID: {productId}");
                }

                var detallesVenta = _detalleVentaService.GetAllByProduct(productId);
                return Ok(detallesVenta);
            }
            return Forbid();
        }

        [HttpGet("{ventaId}")]
        public IActionResult GetAllByVenta([FromRoute] int ventaId)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Forbid();
            }

            var venta = _ventaService.GetById(ventaId);
            if (venta == null)
            {
                return NotFound($"Ninguna venta encontrada con el ID: {ventaId}");
            }

            if (IsUserInRole("Admin") || (IsUserInRole("Client") && userId == venta.ClientId))
            {
                var detallesVenta = _detalleVentaService.GetAllByVenta(ventaId);
                return Ok(detallesVenta);
            }

            return Forbid();
        }

        [HttpPost]
        public IActionResult AddDetalleVenta([FromBody] DetalleVentaDto dto)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Forbid();
            }

            var existingVenta = _ventaService.GetById(dto.VentaId);
            if (existingVenta == null)
            {
                return NotFound($"Ninguna venta encontrada con el ID: {dto.VentaId}");
            }

            if (IsUserInRole("Admin") || (IsUserInRole("Client") && userId == existingVenta.ClientId))
            {
                try
                {
                    var detalleVentaId = _detalleVentaService.AddDetalleVenta(dto);
                    return CreatedAtAction(nameof(GetById), new { id = detalleVentaId }, $"Creado el Detalle de Venta con el ID: {detalleVentaId}");
                }
                catch (NotAllowedException ex)
                {
                    // Manejo específico de la excepción de stock insuficiente
                    return BadRequest(new { message = ex.Message });
                }
                catch (Exception ex)
                {
                    // Manejo de cualquier otra excepción inesperada
                    return StatusCode(500, new { message = "Ocurrió un error inesperado al agregar detalle de venta." });
                }
            }
            return Forbid();
        }


        [HttpPut("{id}")]
        public IActionResult UpdateDetalleVenta([FromRoute] int id, [FromBody] DetalleVentaUpdateRequest request)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Forbid();
            }

            var existingDetalleVenta = _detalleVentaService.GetById(id);
            if (existingDetalleVenta == null)
            {
                return NotFound($"Ningún Detalle de Venta encontrado con el ID: {id}");
            }

            var existingVenta = _ventaService.GetById(existingDetalleVenta.VentaId);
            if (existingVenta == null)
            {
                return NotFound($"Ninguna venta encontrada con el ID: {existingDetalleVenta.VentaId}");
            }

            var isAdmin = IsUserInRole("Admin");
            var isClient = IsUserInRole("Client");

            if (isAdmin || (isClient && userId == existingVenta.ClientId))
            {
                try
                {
                    _detalleVentaService.UpdateDetalleVenta(id, request);
                    return Ok($"Detalle de Venta con ID: {id} actualizado correctamente");
                }
                catch (NotAllowedException ex)
                {
                    return NotFound(ex.Message);
                }
            }

            return Forbid();
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteDetalleVenta([FromRoute] int id)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Forbid();
            }

            var existingDetalleVenta = _detalleVentaService.GetById(id);
            if (existingDetalleVenta == null)
            {
                return NotFound($"Ningún detalle de venta encontrado con el ID: {id}");
            }

            var existingVenta = _ventaService.GetById(existingDetalleVenta.VentaId);
            if (existingVenta == null)
            {
                return NotFound($"Ninguna venta encontrada con el ID: {existingDetalleVenta.VentaId}");
            }

            if (IsUserInRole("Admin") || (IsUserInRole("Client") && userId == existingVenta.ClientId))
            {
                _detalleVentaService.DeleteDetalleVenta(id);
                return Ok($"Eliminado el detalle de venta con ID: {id} ");
            }

            return Forbid();
        }

       
    }
}
