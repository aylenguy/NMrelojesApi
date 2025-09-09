using Application.Interfaces;
using Application.Model;
using Application.Models.Requests;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public DetalleVentaController(
            IDetalleVentaService detalleVentaService,
            IVentaService ventaService)
        {
            _detalleVentaService = detalleVentaService;
            _ventaService = ventaService;
        }

        private bool IsUserInRole(string role)
        {
            var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            return roleClaim != null && roleClaim.Value == role;
        }

        private int? GetUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
                return userId;
            return null;
        }

        private bool CanAccessVenta(int ventaId, int? userId)
        {
            var venta = _ventaService.GetById(ventaId);
            if (venta == null) return false;
            return IsUserInRole("Admin") || (IsUserInRole("Client") && userId == venta.ClientId);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var userId = GetUserId();
            if (userId == null) return Forbid();

            var detalleVenta = _detalleVentaService.GetById(id);
            if (detalleVenta == null)
                return NotFound($"Ningún detalle de venta encontrado con el ID: {id}");

            if (!CanAccessVenta(detalleVenta.VentaId, userId))
                return Forbid();

            // Retornamos DTO listo para front
            return Ok(new
            {
                Id = detalleVenta.Id,
                VentaId = detalleVenta.VentaId,
                ProductId = detalleVenta.ProductId,
                Cantidad = detalleVenta.Quantity,
                PrecioUnitario = detalleVenta.UnitPrice,
                Subtotal = detalleVenta.Subtotal
            });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateDetalleVenta([FromRoute] int id, [FromBody] DetalleVentaUpdateRequest request)
        {
            var userId = GetUserId();
            if (userId == null) return Forbid();

            var detalleVenta = _detalleVentaService.GetById(id);
            if (detalleVenta == null)
                return NotFound($"Ningún Detalle de Venta encontrado con el ID: {id}");

            if (!CanAccessVenta(detalleVenta.VentaId, userId))
                return Forbid();

            try
            {
                _detalleVentaService.UpdateDetalleVenta(id, request);
                return Ok(new { message = $"Detalle de Venta con ID: {id} actualizado correctamente" });
            }
            catch (NotAllowedException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteDetalleVenta([FromRoute] int id)
        {
            var userId = GetUserId();
            if (userId == null) return Forbid();

            var detalleVenta = _detalleVentaService.GetById(id);
            if (detalleVenta == null)
                return NotFound($"Ningún detalle de venta encontrado con el ID: {id}");

            if (!CanAccessVenta(detalleVenta.VentaId, userId))
                return Forbid();

            try
            {
                _detalleVentaService.DeleteDetalleVenta(id);
                return Ok(new { message = $"Eliminado el detalle de venta con ID: {id}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }
    }
}
