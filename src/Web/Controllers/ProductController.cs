﻿using Application.Interfaces;
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

        [HttpGet]
        [Authorize]
        public IActionResult GetAllProducts()
        {
            if (IsUserInRole("Admin") || (IsUserInRole("Client")))
            {
                var products = _productService.GetAllProducts();
                return Ok(products);
            }
            return Forbid();
        }

        [HttpGet("{name}")]
        [Authorize]
        public ActionResult<Product> GetByName([FromRoute] string name)
        {
            if (IsUserInRole("Admin") || IsUserInRole("Client"))
            {
                var product = _productService.Get(name);
                if (product == null)
                {
                    return NotFound($"Producto con el nombre: {name} no encontrado");
                }
                return Ok(product);
            }
            return Forbid();
        }



        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetById([FromRoute] int id)
        {
            if (IsUserInRole("Admin"))
            {
                var product = _productService.Get(id);
                if (product == null)
                {
                    return NotFound($"Producto con el ID: {id} no enctontrado");
                }
                return Ok(product);
            }
            return Forbid();
        }

     

        [HttpPost]
        public IActionResult AddProduct([FromBody] ProductCreateRequest body)
        {
            if (IsUserInRole("Admin"))
            {
                var newProduct = _productService.AddProduct(body);
                return CreatedAtAction(nameof(GetById), new { id = newProduct }, $"Producto creado con el ID: {newProduct}");
            }
            return Forbid();
        }

        [HttpPut("{id}")]
        public ActionResult UpdateProduct([FromRoute] int id, [FromBody] ProductUpdateRequest request)
        {
            if (IsUserInRole("Admin"))
            {
                var existingProduct = _productService.Get(id);
                if (existingProduct == null)
                {
                    return NotFound($"Producto con el ID: {id} no encontrado");
                }
                _productService.UpdateProduct(id, request);
                return Ok($"Producto con ID: {id} actualizado correctamente");
            }
            return Forbid();
        }


        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteProduct([FromRoute] int id)
        {
            if (IsUserInRole("Admin"))
            {
                try
                {
                    _productService.DeleteProduct(id);
                    return Ok($"Producto con el ID: {id} eliminado correctamente.");
                }
                catch (Exception)
                {
                    return StatusCode(400, $"Error al eliminar el producto, tiene ventas asociadas");
                }
            }
            return Forbid();
        }


    }
}

