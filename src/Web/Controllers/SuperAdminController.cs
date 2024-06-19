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
    public class SuperAdminController : ControllerBase
    {
        private readonly ISuperAdminServices _services;

        public SuperAdminController(ISuperAdminServices services)
        {
            _services = services;
        }


        [HttpGet("[action]")]
        public ActionResult<List<SuperAdminDto>> GetAll()
        {
            return _services.GetAll();
        }

        [HttpGet("[action]")]
        public ActionResult<List<SuperAdmin>> GetAllFullData()
        {
            return _services.GetAllFullData();
        }

        [HttpGet("{id}")]
        public ActionResult<SuperAdminDto> GetById([FromRoute] int id)
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

        public ActionResult Create([FromBody] SuperAdminCreateRequest superAdminCreateRequest)
        {
            var newObj = _services.Create(superAdminCreateRequest);

            return CreatedAtAction(nameof(GetById), new { id = newObj.Id }, newObj);
        }

        [HttpPut("{id}")]
        public ActionResult Update([FromRoute] int id, [FromBody] SuperAdminUpdateRequest superAdminUpdateRequest)
        {
            try
            {
                _services.Update(id, superAdminUpdateRequest);
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
