using Application.Interfaces;
using Application.Models.Requests;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        private bool IsUserInRole(string role)
        {
            var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            return roleClaim != null && roleClaim.Value == role;
        }

        // ✅ GET ALL PRODUCTS (Visible para todos)
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAllProducts()
        {
            var products = _productService.GetAllProducts();
            var baseUrl = $"{Request.Scheme}://{Request.Host}/uploads/";

            var updatedProducts = products.Select(p => new
            {
                p.Id,
                Nombre = p.Name,
                Precio = p.Price,
                PrecioAnterior = p.OldPrice,
                Imagen = string.IsNullOrEmpty(p.Image) ? null : baseUrl + p.Image,
                Descripcion = p.Description,
                Color = p.Color,
                Caracteristicas = p.Specs,
                p.Stock
            });

            return Ok(updatedProducts);
        }

        // ✅ GET PRODUCT BY NAME (Visible para todos)
        [HttpGet("{name}")]
        [AllowAnonymous]
        public ActionResult<Product> GetByName([FromRoute] string name)
        {
            var product = _productService.Get(name);
            if (product == null)
                return NotFound($"Producto con el nombre: {name} no encontrado");

            var baseUrl = $"{Request.Scheme}://{Request.Host}/uploads/";
            product.Image = string.IsNullOrEmpty(product.Image) ? null : baseUrl + product.Image;

            return Ok(product);
        }

        // ✅ GET PRODUCT BY ID (Solo Admin)
        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            if (!IsUserInRole("Admin"))
                return Forbid();

            var product = _productService.Get(id);
            if (product == null)
                return NotFound($"Producto con el ID: {id} no encontrado");

            var baseUrl = $"{Request.Scheme}://{Request.Host}/uploads/";
            product.Image = string.IsNullOrEmpty(product.Image) ? null : baseUrl + product.Image;

            return Ok(product);
        }

        // ✅ ADD PRODUCT (Solo Admin)
        [HttpPost]
        public IActionResult AddProduct([FromBody] ProductCreateRequest body)
        {
            if (!IsUserInRole("Admin"))
                return Forbid();

            var newProductId = _productService.AddProduct(body);
            return CreatedAtAction(nameof(GetById), new { id = newProductId }, $"Producto creado con el ID: {newProductId}");
        }

        // ✅ UPDATE PRODUCT (Solo Admin)
        [HttpPut("{id}")]
        public ActionResult UpdateProduct([FromRoute] int id, [FromBody] ProductUpdateRequest request)
        {
            if (!IsUserInRole("Admin"))
                return Forbid();

            var existingProduct = _productService.Get(id);
            if (existingProduct == null)
                return NotFound($"Producto con el ID: {id} no encontrado");

            _productService.UpdateProduct(id, request);
            return Ok($"Producto con ID: {id} actualizado correctamente");
        }

        // ✅ DELETE PRODUCT (Solo Admin)
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct([FromRoute] int id)
        {
            if (!IsUserInRole("Admin"))
                return Forbid();

            try
            {
                _productService.DeleteProduct(id);
                return Ok($"Producto con el ID: {id} eliminado correctamente.");
            }
            catch
            {
                return BadRequest("Error al eliminar el producto, tiene ventas asociadas");
            }
        }
    }
}
