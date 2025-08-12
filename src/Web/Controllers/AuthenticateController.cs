using Application.Interfaces;
using Application.Models.Requests;
using Application.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Domain.Exceptions;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ICustomAuthenticationService _customAuthenticationService;

        public AuthenticateController(IConfiguration config, ICustomAuthenticationService authenticateService)
        {
            _config = config;
            _customAuthenticationService = authenticateService;
        }

        // 🔹 Login Cliente
        [HttpPost("authenticate")]
        public ActionResult<AuthResult> Authenticate([FromBody] CredentialsDtoRequest credentials)
        {
            try
            {
                var result = _customAuthenticationService.Authenticate(credentials);
                return Ok(result); // Siempre devuelve AuthResult { Token, UserType }
            }
            catch (NotAllowedException)
            {
                return Unauthorized(new { message = "Email o contraseña no válidos. Inténtalo de nuevo." });
            }
        }

        // 🔹 Login Admin
        [HttpPost("authenticate-admin")]
        public ActionResult<AuthResult> AuthenticateAdmin([FromBody] CredentialsDtoRequest credentials)
        {
            try
            {
                var result = _customAuthenticationService.AuthenticateAdmin(credentials);
                return Ok(result); // Siempre devuelve AuthResult { Token, UserType }
            }
            catch (NotAllowedException)
            {
                return Unauthorized(new { message = "Email o contraseña de admin no válidos." });
            }
        }
    }
}
