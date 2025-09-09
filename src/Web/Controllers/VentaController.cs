using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;
using System.Collections.Generic;
using Domain.Entities;


namespace Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize] // 🔐 Por defecto todo requiere auth
    public class VentaController : ControllerBase
    {
        private readonly IVentaService _ventaService;
        private readonly EmailService _emailService;
        private readonly IPaymentService _paymentService;

        public VentaController(IVentaService ventaService, EmailService emailService, IPaymentService paymentService)
        {
            _ventaService = ventaService;
            _emailService = emailService;
            _paymentService = paymentService;
        }

        // Helpers
        private bool IsUserInRole(string role)
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            return claim != null && claim.Value == role;
        }

        private int? GetUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim?.Value, out var userId) ? userId : null;
        }

        // 🔹 Método privado para enviar correo de compra
        private void EnviarCorreoCompra(VentaResponseDto venta)
        {
            try
            {
                var productos = venta.Items
      .Select(d => (d.ProductName, d.Quantity, d.UnitPrice))
      .ToList();

                _emailService.EnviarCorreoConfirmacionCompra(
                    venta.CustomerEmail,
                    venta.OrderId.ToString(),
                    productos,
                    venta.Total
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al enviar correo de compra: " + ex.Message);
            }
        }

        [HttpGet("{clientId}")]
        public ActionResult<IEnumerable<VentaResponseDto>> GetAllByClient([FromRoute] int clientId)
        {
            var userId = GetUserId();
            if (userId == null) return Forbid();

            if (!IsUserInRole("Admin") && !(IsUserInRole("Client") && userId == clientId))
                return Forbid();

            var ventas = _ventaService.GetAllByClient(clientId);
            return Ok(ventas);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var venta = _ventaService.GetById(id);
            if (venta == null) return NotFound($"Venta con ID {id} no encontrada");

            var userId = GetUserId();
            if (!IsUserInRole("Admin") && !(IsUserInRole("Client") && userId == venta.ClientId))
                return Forbid();

            return Ok(venta);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult AddVenta([FromBody] VentaDto dto)
        {
            var userId = GetUserId();
            if (userId != null && IsUserInRole("Client"))
            {
                dto.ClientId = userId.Value;
            }

            try
            {
                var ventaId = _ventaService.AddVenta(dto);
                var venta = _ventaService.GetById(ventaId);

                // 🔹 Enviar correo de confirmación al cliente
                EnviarCorreoCompra(venta);

                return CreatedAtAction(nameof(GetById), new { id = ventaId }, venta);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                return BadRequest(new { error = ex.InnerException?.Message ?? ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateVenta([FromRoute] int id, [FromBody] VentaDto dto)
        {
            if (!IsUserInRole("Admin")) return Forbid();

            var existingVenta = _ventaService.GetById(id);
            if (existingVenta == null) return NotFound($"Venta con ID {id} no encontrada");

            try
            {
                _ventaService.UpdateVenta(id, dto);
                var updatedVenta = _ventaService.GetById(id);
                return Ok(updatedVenta);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteVenta([FromRoute] int id)
        {
            var userId = GetUserId();
            if (userId == null) return Forbid();

            var existingVenta = _ventaService.GetById(id);
            if (existingVenta == null) return NotFound($"Venta con ID {id} no encontrada");

            if (IsUserInRole("Admin") || (IsUserInRole("Client") && userId == existingVenta.ClientId))
            {
                _ventaService.DeleteVenta(id);
                return Ok(new { message = $"Venta con ID {id} eliminada" });
            }

            return Forbid();
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public ActionResult<IEnumerable<VentaResponseDto>> GetAll()
        {
            var ventas = _ventaService.GetAll() ?? new List<VentaResponseDto>();
            return Ok(ventas);
        }
        [HttpPost]
        public async Task<IActionResult> CreateFromCart([FromBody] VentaDto dto)
        {
            var userId = GetUserId();
            if (userId == null || !IsUserInRole("Client")) return Forbid();

            try
            {
                // Crear la venta en DB + preferencia de Mercado Pago
                var ventaResponse = await _ventaService.CreateFromCart(userId.Value, dto);

                // Enviar correo de confirmación
                EnviarCorreoCompra(ventaResponse);

                return Ok(ventaResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }



        [HttpPut("{id}/cancel")]
        [Authorize(Roles = "Admin")]
        public IActionResult CancelVenta([FromRoute] int id)
        {
            var venta = _ventaService.GetById(id);
            if (venta == null) return NotFound($"Venta con ID {id} no encontrada.");

            try
            {
                _ventaService.CancelVenta(id);
                return Ok(new { message = $"Venta con ID {id} cancelada y stock actualizado." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateStatus([FromRoute] int id, [FromBody] dynamic body)
        {
            var venta = _ventaService.GetById(id);
            if (venta == null)
                return NotFound($"Venta con ID {id} no encontrada.");

            try
            {
                string statusStr = body?.status?.ToString()?.Trim();
                if (string.IsNullOrEmpty(statusStr))
                    return BadRequest(new { error = "Debe especificar un estado válido" });

                // 🔹 Diccionario para mapear nombres flexibles a los enums
                var statusMap = new Dictionary<string, VentaStatus>(StringComparer.OrdinalIgnoreCase)
        {
            { "Pendiente", VentaStatus.Pendiente },
            { "Enviado", VentaStatus.Enviado },
            { "Entregado", VentaStatus.Entregado },
            { "Cancelado", VentaStatus.Cancelado }
        };

                if (!statusMap.TryGetValue(statusStr, out var newStatus))
                    return BadRequest(new { error = $"Estado '{statusStr}' no es válido" });

                // 🔹 Actualizar estado
                _ventaService.UpdateStatus(id, newStatus);

                var updatedVenta = _ventaService.GetById(id);

                // ✅ Enviar correo según el nuevo estado
                if (newStatus == VentaStatus.Enviado)
                {
                    _emailService.EnviarCorreoPedidoEnviado(
                        updatedVenta.CustomerEmail,
                        updatedVenta.OrderId.ToString()
                     
                    );
                }
                else if (newStatus == VentaStatus.Entregado)
                {
                    _emailService.EnviarCorreoPedidoEntregado(
                        updatedVenta.CustomerEmail,
                        updatedVenta.OrderId.ToString()
                    );
                }

                return Ok(updatedVenta);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpGet("MyOrders")]
        public ActionResult<IEnumerable<VentaResponseDto>> MyOrders()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            // Solo clientes pueden ver sus pedidos
            if (!IsUserInRole("Client")) return Forbid();

            var ventas = _ventaService.GetAllByClient(userId.Value);
            return Ok(ventas);
        }

        [HttpGet("MyOrdersAll")]
        public ActionResult<IEnumerable<VentaResponseDto>> MyOrdersAll()
        {
            var userId = GetUserId();
            var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (userId == null && string.IsNullOrEmpty(emailClaim))
                return Unauthorized();

            var ventas = _ventaService.GetAllByClientOrEmail(userId, emailClaim);
            return Ok(ventas);
        }
        [HttpGet("external/{externalReference}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByExternalReference([FromRoute] string externalReference)
        {
            Console.WriteLine($"[GetByExternalReference] externalReference recibido: '{externalReference}'");
            var venta = await _ventaService.GetByExternalReferenceAsync(externalReference);
            if (venta == null)
            {
                Console.WriteLine($"[GetByExternalReference] No se encontró venta para '{externalReference}'");
                return NotFound($"Venta con referencia {externalReference} no encontrada");
            }
            Console.WriteLine($"[GetByExternalReference] Venta encontrada: OrderId={venta.OrderId}, ExternalReference={venta.ExternalReference}");
            return Ok(venta);
        }



    }
}
