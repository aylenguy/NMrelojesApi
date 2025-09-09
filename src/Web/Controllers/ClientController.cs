using Application.Interfaces;
using Application.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Application.Model.Request;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly ICustomAuthenticationService _authService;
        private readonly EmailService _emailService;


        public ClientController(IClientService clientService, ICustomAuthenticationService authService, EmailService emailService)
        {
            _clientService = clientService;
            _authService = authService;
            _emailService = emailService; // 🔹 asignamos
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] ClientRegisterRequest request)
        {
            try
            {
                var clientId = _clientService.RegisterClient(request);

                try
                {
                    _emailService.EnviarCorreoBienvenida(request.Email); // 👈 manda el mail al correo del cliente
                }
                catch
                {
                    // Si falla el envío, igual no bloquea el registro
                }

                return Ok(new { Message = "Cliente registrado correctamente", ClientId = clientId });
            }
            catch (InvalidOperationException ex)
            {
                // Si el email ya está registrado
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception)
            {
                // Otros errores inesperados
                return StatusCode(500, new { Message = "Ocurrió un error en el registro" });
            }
        }


        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] CredentialsDtoRequest request)
        {
            var result = _authService.Authenticate(request);

            if (!result.Success)
            {
                return BadRequest(result);
                // Ejemplo: { "error": "user_not_found" }
            }

            return Ok(result);
            // Ejemplo: { "token": "xxxx", "userType": "Client" }
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


        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordDto request)
        {
            var client = _clientService.GetByEmail(request.Email);
            if (client == null)
                return BadRequest(new { message = "El correo no está registrado." });

            // Generar token
            var token = Guid.NewGuid().ToString();
            client.ResetToken = token;
            client.ResetTokenExpira = DateTime.UtcNow.AddHours(1);

            _clientService.Update(client);

            // Mandar email
            _emailService.EnviarCorreoRecuperacion(client.Email, token);

            return Ok(new { message = "Se envió un correo con las instrucciones para restablecer la contraseña." });
        }


        [HttpPost("reset-password")]
        [AllowAnonymous]
        public IActionResult ResetPassword([FromBody] ResetPasswordDto request)
        {
            var client = _clientService.GetByToken(request.Token);
            if (client == null || client.ResetTokenExpira < DateTime.UtcNow)
                return BadRequest(new { message = "Token inválido o expirado." });

            client.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            client.ResetToken = null;
            client.ResetTokenExpira = null;

            _clientService.Update(client);

            return Ok(new { message = "La contraseña fue restablecida correctamente." });
        }


    }
}
