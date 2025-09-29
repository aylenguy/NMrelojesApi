using Application.Interfaces;
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
        private readonly IWebHostEnvironment _env;

        public ProductController(IProductService productService, IWebHostEnvironment env)
        {
            _productService = productService;
            _env = env;
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
            var baseUrl = $"{Request.Scheme}://{Request.Host}/uploads/";
            var products = _productService.GetAllProducts();

            var updatedProducts = products.Select(p => new
            {
                p.Id,
                Nombre = p.Name,
                Precio = p.Price,
                PrecioAnterior = p.OldPrice,
                Imagenes = p.Images?.Select(img => baseUrl + img).ToList() ?? new List<string>(), // ✅ lista de URLs
                Descripcion = p.Description,
                Color = p.Color,
                Caracteristicas = p.Specs?.ToList() ?? new List<string>(),
                p.Stock,
                Marca = p.Brand
            });

            return Ok(updatedProducts);
        }

        // ✅ GET PRODUCT BY NAME (Visible para todos)
        [HttpGet("{name}")]
        [AllowAnonymous]
        public IActionResult GetByName([FromRoute] string name)
        {
            var product = _productService.Get(name);
            if (product == null)
                return NotFound($"Producto con el nombre: {name} no encontrado");

            var baseUrl = $"{Request.Scheme}://{Request.Host}/uploads/";

            var productDto = new
            {
                product.Id,
                Nombre = product.Name,
                Precio = product.Price,
                PrecioAnterior = product.OldPrice,
                Imagenes = product.Images?.Select(img => baseUrl + img).ToList() ?? new List<string>(), // ✅ lista de URLs
                Descripcion = product.Description,
                Color = product.Color,
                Caracteristicas = product.Specs?.Split(',').ToList() ?? new List<string>(),
                product.Stock,
                Marca = product.Brand
            };

            return Ok(productDto);
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

            var productDto = new
            {
                product.Id,
                Nombre = product.Name,
                Precio = product.Price,
                PrecioAnterior = product.OldPrice,
                Imagenes = product.Images?.Select(img => baseUrl + img).ToList() ?? new List<string>(),
                Descripcion = product.Description,
                Color = product.Color,
                Caracteristicas = product.Specs?.Split(',').ToList() ?? new List<string>(),
                product.Stock,
                Marca = product.Brand
            };

            return Ok(productDto);
        }

        // ✅ ADD PRODUCT con IFormFile (Solo Admin)
        [HttpPost]
        public IActionResult AddProduct([FromForm] ProductCreateRequest request, List<IFormFile> imageFiles)
        {
            if (!IsUserInRole("Admin"))
                return Forbid();

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileNames = new List<string>();

            if (imageFiles != null && imageFiles.Count > 0)
            {
                foreach (var imageFile in imageFiles)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }

                    fileNames.Add(fileName);
                }
            }

            var newProductId = _productService.AddProduct(request, fileNames);

            var imageUrls = fileNames.Select(fn => $"{Request.Scheme}://{Request.Host}/uploads/{fn}").ToList();

            return CreatedAtAction(nameof(GetById),
                new { id = newProductId },
                new
                {
                    message = "Producto creado correctamente",
                    id = newProductId,
                    imageUrls
                });
        }


        [HttpPut("{id}")]
        public ActionResult UpdateProduct([FromRoute] int id, [FromBody] ProductUpdateRequest request)
        {
            if (!IsUserInRole("Admin"))
                return Forbid();

            var existingProduct = _productService.Get(id);
            if (existingProduct == null)
                return NotFound($"Producto con el ID: {id} no encontrado");

            _productService.UpdateProduct(id, request);
            return Ok(new { message = $"Producto con ID: {id} actualizado correctamente" });
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
