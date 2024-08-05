using Application.Interfaces;
using Application.Model.Request;
using ConsultaAlumnos.Application.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ProductDto>> GetAll()
        {
            return Ok(_productService.GetAll());
        }

        [HttpGet("{id}")]
        public ActionResult<ProductDto> GetById(int id)
        {
            var product = _productService.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        public ActionResult<ProductDto> Create(ProductCreateRequest request)
        {
            var product = _productService.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, ProductDto.Create(product));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, ProductUpdateRequest request)
        {
            var existingProduct = _productService.GetById(id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            _productService.Update(id, request);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existingProduct = _productService.GetById(id);
            if (existingProduct == null)
            {
                return NotFound();
            }
            _productService.Delete(id);
            return NoContent();
        }
    }
}

