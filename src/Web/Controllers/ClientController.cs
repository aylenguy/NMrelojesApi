using Application.Interfaces;
using Application.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly ICustomAuthenticationService _authService;

        public ClientController(IClientService clientService, ICustomAuthenticationService authService)
        {
            _clientService = clientService;
            _authService = authService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] ClientRegisterRequest request)
        {
            var clientId = _clientService.RegisterClient(request);
            return Ok(new { Message = "Cliente registrado correctamente", ClientId = clientId });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] CredentialsDtoRequest request)
        {
            var token = _authService.Authenticate(request);
            return Ok(new { token }); // 🔹 minúscula para frontend
        }

        [HttpGet("profile")]
        [Authorize(Roles = "Client")]
        public IActionResult Profile()
        {
            var userId = User.FindFirst("sub")?.Value;
            var email = User.FindFirst("email")?.Value;
            var username = User.FindFirst("username")?.Value;

            return Ok(new { Id = userId, Email = email, UserName = username });
        }
    }
}
