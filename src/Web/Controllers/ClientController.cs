using Application.Interfaces;
using Application.Models.Requests;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;


namespace Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _service;

        public ClientController(IClientService service)
        {
            _service = service;
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


        [HttpGet]
        [Authorize]
        public IActionResult GetAll()
        {
            if (IsUserInRole("Admin"))
            {
                return Ok(_service.GetAllClients());
            }
            // Si el rol no es Admin o el claim no existe, prohibir acceso
            return Forbid();
        }

        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetById([FromRoute] int id)
        {
            if (IsUserInRole("Admin"))
            {
                var client = _service.Get(id);
                if (client == null)
                {
                    return NotFound($"Ningún cliente encontrado con el ID: {id}");
                }
                return Ok(client);
            }
            return Forbid();
        }

        [HttpGet("{lastName}")]
        [Authorize]
        public IActionResult GetByLastName([FromRoute] string lastName)
        {
            if (IsUserInRole("Admin"))
            {
                var client = _service.GetByLastName(lastName); 
                if (client == null)
                {
                    return NotFound($"Ningún cliente encontrado con el apellido: {lastName}");
                }
                return Ok(client);
            }
            return Forbid();
        }

        [HttpPost]
        
        public IActionResult Add([FromBody] ClientCreateRequest body)
        {
            var newClient = _service.AddClient(body);
            return CreatedAtAction(nameof(GetById), new { id = newClient }, $"Creado el Cliente con el ID: {newClient}");
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteClient([FromRoute] int id)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Forbid();
            }

            var clientExisting = _service.Get(id);
            if (clientExisting == null)
            {
                return NotFound($"Ningún Cliente encontrado con el ID: {id}");
            }

            if (IsUserInRole("Admin") || (IsUserInRole("Client") && userId == id))
            {
                _service.DeleteClient(id);
                return Ok($"Eliminado el Cliente con ID: {id} ");
            }

            return Forbid();
        }

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateClient([FromRoute] int id, [FromBody] ClientUpdateRequest request)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Forbid();
            }

            var clientExisting = _service.Get(id);
            if (clientExisting == null)
            {
                return NotFound($"Ningún Cliente encontrado con el ID: {id}");
            }

            if (IsUserInRole("Admin") || (IsUserInRole("Client") && userId == id))
            {
                _service.UpdateClient(id, request);
                return Ok($"Cliente con ID: {id} actualizado correctamente");
            }

            return Forbid();
        }
    }
}