using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly IVentaService _ventaService;

        public VentasController(IVentaService ventaService)
        {
            _ventaService = ventaService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Venta>> GetAll()
        {
            return Ok(_ventaService.GetAllVentas());
        }

        [HttpGet("{id}")]
        public ActionResult<Venta> GetById(int id)
        {
            var venta = _ventaService.GetVentaById(id);
            if (venta == null)
            {
                return NotFound();
            }
            return Ok(venta);
        }

        [HttpPost]
        public ActionResult<Venta> Create(Venta venta)
        {
            _ventaService.AddVenta(venta);
            return CreatedAtAction(nameof(GetById), new { id = venta.Id }, venta);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Venta venta)
        {
            if (id != venta.Id)
            {
                return BadRequest();
            }
            _ventaService.UpdateVenta(venta);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existingVenta = _ventaService.GetVentaById(id);
            if (existingVenta == null)
            {
                return NotFound();
            }
            _ventaService.DeleteVenta(id);
            return NoContent();
        }
    }
}
