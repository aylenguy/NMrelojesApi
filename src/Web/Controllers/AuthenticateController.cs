using Application.Interfaces;
using Application.Models.Requests;
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

        [HttpPost("authenticate")] //usamos POST para enviar los datos para hacer el login
        public ActionResult<string> Authenticate([FromBody] CredentialsDtoRequest credentials)
        {
            try
            {
                // validamos los parámetros que enviamos.
                string token = _customAuthenticationService.Authenticate(credentials); 
                return Ok(token);
            }
            catch (NotAllowedException)
            {
                return Unauthorized(new { message = "Email o contraseña no válidos.Inténtalo de nuevo" });
            }
        }
    }
}