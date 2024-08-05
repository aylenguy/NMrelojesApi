using Application.Interfaces;
using Application.Model;
using Application.Model.Request;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminsController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<AdminDto>> GetAll()
        {
            return Ok(_adminService.GetAll());
        }

        [HttpGet("{id}")]
        public ActionResult<AdminDto> GetById(int id)
        {
            var admin = _adminService.GetById(id);
            if (admin == null)
            {
                return NotFound();
            }
            return Ok(admin);
        }

        [HttpPost]
        public ActionResult<AdminDto> Create(AdminCreateRequest request)
        {
            var admin = _adminService.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = admin.Id }, admin);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, AdminUpdateRequest request)
        {
            var existingAdmin = _adminService.GetById(id);
            if (existingAdmin == null)
            {
                return NotFound();
            }
            _adminService.Update(id, request);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existingAdmin = _adminService.GetById(id);
            if (existingAdmin == null)
            {
                return NotFound();
            }
            _adminService.Delete(id);
            return NoContent();
        }
    }
}

