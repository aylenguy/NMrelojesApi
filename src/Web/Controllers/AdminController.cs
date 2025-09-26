using Application.Interfaces;
using Application.Model;
using Application.Models.Requests;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _service;

        public AdminController(IAdminService service)
        {
            _service = service;
        }

        // ---------------------- Admins ----------------------
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_service.GetAllAdmins());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var admin = _service.Get(id);
            if (admin == null)
                return NotFound($"No se encontró admin con ID: {id}");
            return Ok(admin);
        }

        [HttpGet("name/{name}")]
        public IActionResult GetByName(string name)
        {
            var admin = _service.Get(name);
            if (admin == null)
                return NotFound($"No se encontró admin con nombre: {name}");
            return Ok(admin);
        }

        [HttpPost]
        public IActionResult AddAdmin([FromBody] AdminCreateRequest body)
        {
            var newAdminId = _service.AddAdmin(body);
            return CreatedAtAction(nameof(GetById), new { id = newAdminId }, $"Admin creado con ID: {newAdminId}");
        }

        [HttpPut("{id}")]
        public IActionResult UpdateAdmin(int id, [FromBody] AdminUpdateRequest request)
        {
            if (_service.Get(id) == null)
                return NotFound($"No se encontró admin con ID: {id}");

            _service.UpdateAdmin(id, request);
            return Ok($"Admin con ID: {id} actualizado correctamente");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAdmin(int id)
        {
            if (_service.Get(id) == null)
                return NotFound($"No se encontró admin con ID: {id}");

            _service.DeleteAdmin(id);
            return Ok($"Admin con ID: {id} eliminado correctamente");
        }

        // ---------------------- Users ----------------------
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            return Ok(_service.GetAllUsers());
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            if (!_service.DeleteUser(id))
                return NotFound("Usuario no encontrado");
            return Ok("Usuario eliminado correctamente");
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUserRole(int id, [FromBody] UpdateUserRoleDto dto)
        {
            if (!_service.UpdateUserRole(id, dto.UserType))
                return NotFound("Usuario no encontrado");
            return Ok("Rol actualizado correctamente");
        }
    }
}

