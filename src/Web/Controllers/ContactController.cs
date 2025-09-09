using Application.Model;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ContactoController : ControllerBase
{
    private readonly EmailService _emailService;

    public ContactoController(EmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost]
    public async Task<IActionResult> EnviarCorreo([FromBody] ContactoDto dto)
    {
        try
        {
            string subject = $"📩 Nuevo mensaje de contacto - {dto.Nombre}";
            string body = $@"
Nombre: {dto.Nombre}
Email: {dto.Email}
Teléfono: {dto.Telefono}

Mensaje:
{dto.Mensaje ?? "(sin mensaje)"}
";

            // Se lo enviamos a tu correo (configurado en EmailSettings)
            await _emailService.SendEmailAsync(subject, body, dto.Email);

            return Ok(new { message = "✅ Correo enviado correctamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "❌ Error al enviar el correo", error = ex.Message });
        }
    }
}