using Application.Interfaces;
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
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _service;

        public AdminController(IAdminService service)
        {
            _service = service;

        }

        private bool IsUserInRole(string role)
        {
            var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role); // Obtener el claim de rol, si existe
            return roleClaim != null && roleClaim.Value == role; //Verificar si el claim existe y su valor es "role"
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            if (IsUserInRole("Admin"))
            {
                return Ok(_service.GetAllAdmins());
            }
            return Forbid();
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            if (IsUserInRole("Admin"))
            {
                var admin = _service.Get(id);
                if (admin == null)
                {
                    return NotFound($"No se encontró ningún admin con el ID: {id}");
                }
                return Ok(admin);
            }
            return Forbid();
        }

        [HttpGet("{name}")]
        public IActionResult GetByName([FromRoute] string name)
        {
            if (IsUserInRole("Admin"))
            {
                var admin = _service.Get(name);
                if (admin == null)
                {
                    return NotFound($"No se encontró ningún admin con el nombre: {name}");
                }
                return Ok(admin);
            }
            return Forbid();
        }

        [HttpPost]
        public IActionResult Add([FromBody] AdminCreateRequest body)
        {
            if (IsUserInRole("Admin"))
            {
                var newAdmin = _service.AddAdmin(body);
                return CreatedAtAction(nameof(GetById), new { id = newAdmin }, $"Creado el Admin con el ID: {newAdmin}");
            }
            return Forbid();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAdmin(int id)
        {
            if (IsUserInRole("Admin"))
            {
                var existingAdmin = _service.Get(id);
                if (existingAdmin == null)
                {
                    return NotFound($"No se encontró ningún Admin con el ID: {id}");
                }
                _service.DeleteAdmin(id);
                return Ok($"Admin con ID: {id} eliminado");
            }
            return Forbid();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateAdmin([FromRoute] int id, [FromBody] AdminUpdateRequest request)
        {
            if (IsUserInRole("Admin"))
            {
                var existingAdmin = _service.Get(id);
                if (existingAdmin == null)
                {
                    return NotFound($"No se encontró ningún Admin con el ID: {id}");
                }
                _service.UpdateAdmin(id, request);
                return Ok($"Admin con ID: {id} actualizado correctamente");
            }
            return Forbid();
        }
    }
}