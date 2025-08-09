using Application.Interfaces;
using Application.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ICustomAuthenticationService _authService;

        public AuthController(ICustomAuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpPost("AdminLogin")]
        [AllowAnonymous]
        public IActionResult AdminLogin([FromBody] CredentialsDtoRequest request)
        {
            try
            {
                var token = _authService.AuthenticateAdmin(request);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}
