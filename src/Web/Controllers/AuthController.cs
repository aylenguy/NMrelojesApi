using Application.Interfaces;
using Application.Models.Requests;
using Application.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.Exceptions;

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

        // 🔹 Login Admin
        [HttpPost("admin-login")]
        [AllowAnonymous]
        public ActionResult<AuthResult> AdminLogin([FromBody] CredentialsDtoRequest request)
        {
            try
            {
                var result = _authService.AuthenticateAdmin(request);
                return Ok(result); // AuthResult { Token, UserType }
            }
            catch (NotAllowedException)
            {
                return Unauthorized(new { message = "Email o contraseña de admin no válidos." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
