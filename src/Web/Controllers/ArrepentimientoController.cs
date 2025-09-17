using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ArrepentimientoController : ControllerBase
{
    private readonly EmailService _emailService;

    public ArrepentimientoController(EmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost]
    public IActionResult EnviarSolicitud([FromBody] ArrepentimientoRequest request)
    {
        _emailService.EnviarCorreoArrepentimiento(
            request.Email,
            request.Nombre,
            request.Telefono,
            request.CodigoCompra,
            request.Inconveniente
        );

        return Ok(new { mensaje = "Solicitud enviada correctamente" });
    }
}

public class ArrepentimientoRequest
{
    public string Nombre { get; set; }
    public string Telefono { get; set; }
    public string Email { get; set; }
  
    public string Inconveniente { get; set; }
}
