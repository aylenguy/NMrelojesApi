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
            // Obtener el claim de rol y verificar si existe
            var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role); 
            return roleClaim != null && roleClaim.Value == role; 
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

        [HttpGet("{name}")]
        public ActionResult<Admin> GetByName([FromRoute] string name)
        {
            if (IsUserInRole("Admin"))
            {
                var admin = _service.Get(name);
                if (admin == null)
                {
                    return NotFound($"Ningún admin encontrado con el nombre: {name}");
                }
                return Ok(admin);
            }
            return Forbid();
        }

        [HttpGet("{id}")]
        public ActionResult<Admin> GetById([FromRoute] int id)
        {
            if (IsUserInRole("Admin"))
            {
                var admin = _service.Get(id);
                if (admin == null)
                {
                    return NotFound($"Ningún admin encontrado con el ID: {id}");
                }
                return Ok(admin);
            }
            return Forbid();
        }



        [HttpPost]
        public IActionResult AddAdmin([FromBody] AdminCreateRequest body)
        {
            if (IsUserInRole("Admin"))
            {
                var newAdmin = _service.AddAdmin(body);
                return CreatedAtAction(nameof(GetById), new { id = newAdmin }, $"Admin creado con el ID: {newAdmin}");
            }
            return Forbid();
        }


        [HttpPut("{id}")]
        public IActionResult UpdateAdmin([FromRoute] int id, [FromBody] AdminUpdateRequest request)
        {
            if (IsUserInRole("Admin"))
            {
                var adminexisting = _service.Get(id);
                if (adminexisting == null)
                {
                    return NotFound($"Ningún Admin encontrado con el ID: {id}");
                }
                _service.UpdateAdmin(id, request);
                return Ok($"Admin con ID: {id} actualizado correctamente");
            }
            return Forbid();
        }


        [HttpDelete("{id}")]
        public ActionResult DeleteAdmin(int id)
        {
            if (IsUserInRole("Admin"))
            {
                var adminexisting = _service.Get(id);
                if (adminexisting == null)
                {
                    return NotFound($"Ningún admin encontrado con el ID: {id}");
                }
                _service.DeleteAdmin(id);
                return Ok($"Eliminado el Admin con ID: {id}");
            }
            return Forbid();
        }

    }
}

