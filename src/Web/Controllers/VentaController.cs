using Application.Interfaces;
using Application.Models;
using Application.Models.Requests;
using Application.Services;
using Domain.Entities;
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
    public class VentaController : ControllerBase
    {
        private readonly IVentaService _ventaService;

        public VentaController(IVentaService ventaService)
        {
            _ventaService = ventaService;
        }

        private bool IsUserInRole(string role)
        {
            var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role); // Obtener el claim de rol, si existe
            return roleClaim != null && roleClaim.Value == role; //Verificar si el claim existe y su valor es "role"
        }
        private int? GetUserId() //Funcion para obtener el userId de las claims del usuario autenticado en el contexto de la solicitud actual.
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }
            return null;
        }

        [HttpGet("{clientId}")]
        public IActionResult GetAllByClient([FromRoute] int clientId)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Forbid();
            }
            if (IsUserInRole("Admin") || (IsUserInRole("Client") && userId == clientId))
            {
                var ventas = _ventaService.GetAllByClient(clientId);
                return Ok(ventas);
            }
            return Forbid();
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            if (IsUserInRole("Admin"))
            {
                var venta = _ventaService.GetById(id);
                if (venta == null)
                {
                    return NotFound($"No se encontró ninguna venta con el ID: {id}");
                }
                return Ok(venta);
            }
            return Forbid();
        }

        [HttpPost]
        public IActionResult Add([FromBody] VentaDto dto)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Forbid();
            }
            if (IsUserInRole("Admin") || (IsUserInRole("Client") && userId == dto.ClientId))
            {
                var ventaId = _ventaService.AddVenta(dto);
                return CreatedAtAction(nameof(GetById), new { id = ventaId }, $"Creada la Venta con el ID: {ventaId}");
            }
            return Forbid();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteVenta([FromRoute] int id)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Forbid();
            }
            var existingVenta = _ventaService.GetById(id);
            if (existingVenta == null)
            {
                return NotFound($"No se encontró ninguna venta con el ID: {id}");
            }

            if (IsUserInRole("Admin") || (IsUserInRole("Client") && userId == existingVenta.ClientId))
            {
                _ventaService.DeleteVenta(id);
                return Ok($"Venta con ID: {id} eliminada");
            }

            return Forbid();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateVenta([FromRoute] int id, [FromBody] VentaDto dto)
        {
            if (IsUserInRole("Admin"))
            {
                var existingVenta = _ventaService.GetById(id);
                if (existingVenta == null)
                {
                    return NotFound($"No se encontró ninguna Venta con el ID: {id}");
                }

                _ventaService.UpdateVenta(id, dto);
                return Ok($"Venta con ID: {id} actualizada correctamente");
            }
            return Forbid();
        }
    }
}