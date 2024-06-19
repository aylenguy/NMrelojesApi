using Application.Interfaces;
using Application.Model.Request;
using Application.Model;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ConsultaAlumnos.Application.Models;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductServices _services;

        public ProductController(IProductServices services)
        {
            _services = services;
        }


        [HttpGet("[action]")]
        public ActionResult<List<ProductDto>> GetAll()
        {
            return _services.GetAll();
        }

        [HttpGet("[action]")]
        public ActionResult<List<Product>> GetAllFullData()
        {
            return _services.GetAllFullData();
        }

        [HttpGet("{id}")]
        public ActionResult<ProductDto> GetById([FromRoute] int id)
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

        public ActionResult Create([FromBody] ProductCreateRequest productCreateRequest)
        {
            var newObj = _services.Create(productCreateRequest);

            return CreatedAtAction(nameof(GetById), new { id = newObj.Id }, newObj);
        }

        [HttpPut("{id}")]
        public ActionResult Update([FromRoute] int id, [FromBody] ProductUpdateRequest productUpdateRequest)
        {
            try
            {
                _services.Update(id, productUpdateRequest);
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
