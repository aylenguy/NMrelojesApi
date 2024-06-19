using Application.Interfaces;
using Application.Model;
using Application.Model.Request;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminServices _services;

        public AdminController(IAdminServices services)
        {
            _services = services;
        }


        [HttpGet("[action]")]
        public ActionResult<List<AdminDto>> GetAll()
        {
            return _services.GetAll();
        }

        [HttpGet("[action]")]
        public ActionResult<List<Admin>> GetAllFullData()
        {
            return _services.GetAllFullData();
        }

        [HttpGet("{id}")]
        public ActionResult<AdminDto> GetById([FromRoute] int id)
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

        public ActionResult Create([FromBody] AdminCreateRequest adminCreateRequest)
        {
            var newObj = _services.Create(adminCreateRequest);

            return CreatedAtAction(nameof(GetById),new {id = newObj.Id}, newObj);
        }

        [HttpPut("{id}")]
        public ActionResult Update([FromRoute] int id,[FromBody] AdminUpdateRequest adminUpdateRequest)
        {
            try
            {
                _services.Update(id, adminUpdateRequest);
                return NoContent();
            }
            catch(NotFoundException ex)
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
            catch( NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
