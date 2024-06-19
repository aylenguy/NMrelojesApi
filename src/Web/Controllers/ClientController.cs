using Application.Interfaces;
using Application.Model.Request;
using Application.Model;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientServices _services;

        public ClientController(IClientServices services)
        {
            _services = services;
        }


        [HttpGet("[action]")]
        public ActionResult<List<ClientDto>> GetAll()
        {
            return _services.GetAll();
        }

        [HttpGet("[action]")]
        public ActionResult<List<Client>> GetAllFullData()
        {
            return _services.GetAllFullData();
        }

        [HttpGet("{id}")]
        public ActionResult<ClientDto> GetById([FromRoute] int id)
        {
            try
            {
                return _services.GetById(id);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPost]

        public ActionResult Create([FromBody] ClientCreateRequest clientCreateRequest)
        {
            var newObj = _services.Create(clientCreateRequest);

            return CreatedAtAction(nameof(GetById), new { id = newObj.Id }, newObj);
        }

        [HttpPut("{id}")]
        public ActionResult Update([FromRoute] int id, [FromBody] ClientUpdateRequest clientUpdateRequest)
        {
            try
            {
                _services.Update(id, clientUpdateRequest);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            try
            {
                _services.Delete(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
