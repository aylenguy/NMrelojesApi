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
            var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role); 
            return roleClaim != null && roleClaim.Value == role; 
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
        public ActionResult<IEnumerable<Venta>> GetAllByClient([FromRoute] int clientId)
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
                    return NotFound($"Venta con ID {id} no encontrada");
                }
                return Ok(venta);
            }
            return Forbid();
        }

        [HttpPost]
        public IActionResult AddVenta([FromBody] VentaDto dto)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Forbid();
            }

            if (IsUserInRole("Admin"))
            {
                // Si es admin, puede crear venta para cualquier cliente
                var ventaId = _ventaService.AddVenta(dto);
                return CreatedAtAction(nameof(GetById), new { id = ventaId }, $"Venta creada con el ID: {ventaId}");
            }

            if (IsUserInRole("Client"))
            {
                // Sobrescribir ClientId con el del token
                dto.ClientId = userId.Value;

                var ventaId = _ventaService.AddVenta(dto);
                return CreatedAtAction(nameof(GetById), new { id = ventaId }, $"Venta creada con el ID: {ventaId}");
            }

            return Forbid();
        }


        [HttpPut("{id}")]
        public ActionResult UpdateVenta([FromRoute] int id, [FromBody] VentaDto dto)
        {
            if (IsUserInRole("Admin"))
            {
                var existingVenta = _ventaService.GetById(id);
                if (existingVenta == null)
                {
                    return NotFound($"Venta con ID {id} no encontrada");
                }

                _ventaService.UpdateVenta(id, dto);
                return Ok($"Venta con ID: {id} actualizada correctamente");
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
                return NotFound($"Venta con ID {id} no encontrada");
            }

            if (IsUserInRole("Admin") || (IsUserInRole("Client") && userId == existingVenta.ClientId))
            {
                _ventaService.DeleteVenta(id);
                return Ok($"Venta con ID: {id} eliminada");
            }

            return Forbid();
        }

        
    }
}