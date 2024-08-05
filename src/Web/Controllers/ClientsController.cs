using Application.Interfaces;
using Application.Model;
using Application.Model.Request;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ClientDto>> GetAll()
        {
            return Ok(_clientService.GetAll());
        }

        [HttpGet("{id}")]
        public ActionResult<ClientDto> GetById(int id)
        {
            var client = _clientService.GetById(id);
            if (client == null)
            {
                return NotFound();
            }
            return Ok(client);
        }

        [HttpPost]
        public ActionResult<ClientDto> Create(ClientCreateRequest request)
        {
            var client = _clientService.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, ClientUpdateRequest request)
        {
            var existingClient = _clientService.GetById(id);
            if (existingClient == null)
            {
                return NotFound();
            }
            _clientService.Update(id, request);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existingClient = _clientService.GetById(id);
            if (existingClient == null)
            {
                return NotFound();
            }
            _clientService.Delete(id);
            return NoContent();
        }
    }
}
