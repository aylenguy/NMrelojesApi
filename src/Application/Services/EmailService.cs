using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Application.Model;

public class EmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public void EnviarCorreoBienvenida(string destinatario)
    {
        try
        {
            var fromAddress = new MailAddress(_settings.SenderEmail, _settings.SenderName);
            var toAddress = new MailAddress(destinatario);

            string subject = "¡Bienvenido a NM Relojes!";
            string body = @"
<!DOCTYPE html>
<html>
<head>
  <meta charset='UTF-8'>
  <style>
    body { font-family: Arial, sans-serif; background-color: #f5f5f5; margin: 0; padding: 0; }
    .container { max-width: 600px; margin: 30px auto; background: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 12px rgba(0,0,0,0.1); }
    .header { background: #000000; padding: 20px; text-align: center; }
    .header img { max-height: 60px; }
    .content { padding: 30px; text-align: center; }
    .content h2 { color: #333333; }
    .content p { color: #555555; line-height: 1.5; }
    .btn { display: inline-block; margin-top: 20px; padding: 12px 24px; background: #000000; color: #ffffff !important; text-decoration: none; border-radius: 8px; font-weight: bold; }
    .footer { background: #f0f0f0; padding: 15px; text-align: center; font-size: 12px; color: #777777; }
  </style>
</head>
<body>
  <div class='container'>
    <div class='header'>
      <img src='cid:LogoNM' alt='NM Relojes'>
    </div>
    <div class='content'>
      <h2>¡Bienvenido a <b>NM Relojes</b>!</h2>
      <p>Gracias por registrarte en nuestra tienda online.<br>
      Ahora formas parte de nuestra comunidad de amantes de los relojes.</p>
      <p><b>Explora nuestras ofertas exclusivas y encuentra tu próximo reloj favorito.</b></p>
      <a href='https://nmrelojes.com' class='btn'>Ir a la tienda</a>
    </div>
    <div class='footer'>
      © 2025 NM Relojes - Todos los derechos reservados
    </div>
  </div>
</body>
</html>";

            // Crear la vista HTML con el logo embebido
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/logo.jpeg");

            if (File.Exists(logoPath))
            {
                LinkedResource logo = new LinkedResource(logoPath);
                logo.ContentId = "LogoNM"; // debe coincidir con el src del <img>
                htmlView.LinkedResources.Add(logo);
            }

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                IsBodyHtml = true
            })
            {
                message.AlternateViews.Add(htmlView);

                using (var smtp = new SmtpClient
                {
                    Host = _settings.SmtpServer,
                    Port = _settings.Port,
                    EnableSsl = _settings.EnableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_settings.SenderEmail, _settings.Password)
                })
                {
                    smtp.Send(message);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al enviar correo: " + ex.Message);
        }
    }

    public void EnviarCorreoConfirmacionCompra(string destinatario, string numeroPedido, List<(string nombreProducto, int cantidad, decimal precio)> productos, decimal total)
    {
        try
        {
            var fromAddress = new MailAddress(_settings.SenderEmail, _settings.SenderName);
            var toAddress = new MailAddress(destinatario);

            string subject = $"Confirmación de tu compra #{numeroPedido}";

            // Construimos la tabla de productos en HTML
            string tablaProductos = "<table style='width:100%; border-collapse: collapse; margin-top:10px;'>";
            tablaProductos += "<tr><th style='border-bottom:1px solid #ddd; text-align:left;'>Producto</th><th style='border-bottom:1px solid #ddd; text-align:center;'>Cantidad</th><th style='border-bottom:1px solid #ddd; text-align:right;'>Precio</th></tr>";

            foreach (var p in productos)
            {
                tablaProductos += $"<tr><td>{p.nombreProducto}</td><td style='text-align:center'>{p.cantidad}</td><td style='text-align:right'>${p.precio:F2}</td></tr>";
            }

            tablaProductos += $"<tr><td colspan='2' style='text-align:right; font-weight:bold;'>Total:</td><td style='text-align:right; font-weight:bold;'>${total:F2}</td></tr>";
            tablaProductos += "</table>";

            // Cuerpo del correo HTML con cid para el logo
            string body = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: Arial, sans-serif; background-color: #f5f5f5; margin:0; padding:0; }}
        .container {{ max-width:600px; margin:20px auto; background:white; border-radius:12px; overflow:hidden; padding:20px; box-shadow:0 4px 12px rgba(0,0,0,0.1); }}
        .header {{ background:#000; padding:20px; text-align:center; }}
        .header img {{ max-height:60px; }}
        h2 {{ color:#333; }}
        p {{ color:#555; line-height:1.5; }}
        .btn {{ display:inline-block; margin-top:20px; padding:12px 24px; background:#000; color:#fff !important; text-decoration:none; border-radius:8px; font-weight:bold; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <img src='cid:LogoNM' alt='NM Relojes'>
        </div>
        <div class='content'>
            <h2>¡Gracias por tu compra!</h2>
            <p>Tu pedido <b>#{numeroPedido}</b> ha sido recibido y estamos procesándolo.</p>
            {tablaProductos}
            <p>Pronto recibirás más información sobre el envío.</p>
            <a href='https://nmrelojes.com' class='btn'>Ir a la tienda</a>
        </div>
    </div>
</body>
</html>";

            // Crear la vista HTML con logo embebido
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
            var logoPath = Path.Combine(AppContext.BaseDirectory, "wwwroot/uploads/logo.jpeg");

            if (File.Exists(logoPath))
            {
                LinkedResource logo = new LinkedResource(logoPath);
                logo.ContentId = "LogoNM"; // debe coincidir con el cid del <img>
                htmlView.LinkedResources.Add(logo);
            }

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                IsBodyHtml = true
            })
            {
                message.AlternateViews.Add(htmlView);

                using (var smtp = new SmtpClient
                {
                    Host = _settings.SmtpServer,
                    Port = _settings.Port,
                    EnableSsl = _settings.EnableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_settings.SenderEmail, _settings.Password)
                })
                {
                    smtp.Send(message);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al enviar correo de compra: " + ex.Message);
        }
    }

    public void EnviarCorreoRecuperacion(string destinatario, string token)
    {
        var resetLink = $"http://localhost:5173/reset-password?token={token}";
        string subject = "Recupera tu contraseña - NM Relojes";

        // HTML con cid para el logo
        string body = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: Arial, sans-serif; background:#f5f5f5; }}
        .container {{ max-width:600px; margin:30px auto; background:#fff; border-radius:12px; padding:20px; box-shadow:0 4px 12px rgba(0,0,0,0.1); }}
        .header {{ background:#000; padding:20px; text-align:center; }}
        .header img {{ max-height:60px; }}
        .content {{ padding:30px; text-align:center; }}
        .btn {{ display:inline-block; margin-top:20px; padding:12px 24px; background:#000; color:#fff !important; text-decoration:none; border-radius:8px; font-weight:bold; }}
        .footer {{ margin-top:20px; font-size:12px; color:#777; text-align:center; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <img src='cid:LogoNM' alt='NM Relojes'>
        </div>
        <div class='content'>
            <h2>¿Olvidaste tu contraseña?</h2>
            <p>Parece que solicitaste restablecer tu contraseña.</p>
            <a href='{resetLink}' class='btn'>Restablecer contraseña</a>
            <p style='margin-top:20px; color:#999;'>Si no fuiste vos, puedes ignorar este mensaje.</p>
        </div>
        <div class='footer'>
            © 2025 NM Relojes - Todos los derechos reservados
        </div>
    </div>
</body>
</html>";

        // Crear la vista HTML con logo embebido
        AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
        var logoPath = Path.Combine(AppContext.BaseDirectory, "wwwroot/uploads/logo.jpeg");

        if (File.Exists(logoPath))
        {
            LinkedResource logo = new LinkedResource(logoPath);
            logo.ContentId = "LogoNM"; // debe coincidir con el cid del <img>
            htmlView.LinkedResources.Add(logo);
        }

        var fromAddress = new MailAddress(_settings.SenderEmail, _settings.SenderName);

        using (var message = new MailMessage(fromAddress, new MailAddress(destinatario))
        {
            Subject = subject,
            IsBodyHtml = true
        })
        {
            message.AlternateViews.Add(htmlView);

            using (var smtp = new SmtpClient
            {
                Host = _settings.SmtpServer,
                Port = _settings.Port,
                EnableSsl = _settings.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_settings.SenderEmail, _settings.Password)
            })
            {
                smtp.Send(message);
            }
        }
    }


    public async Task SendEmailAsync(string subject, string body, string? replyTo = null)
    {
        var fromAddress = new MailAddress(_settings.SenderEmail, _settings.SenderName);

        // Crear la vista HTML con logo embebido
        AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
        var logoPath = Path.Combine(AppContext.BaseDirectory, "wwwroot/uploads/logo.jpeg");

        if (File.Exists(logoPath))
        {
            LinkedResource logo = new LinkedResource(logoPath);
            logo.ContentId = "LogoNM"; // debe coincidir con el cid del <img>
            htmlView.LinkedResources.Add(logo);
        }

        var mailMessage = new MailMessage
        {
            From = fromAddress,
            Subject = subject,
            IsBodyHtml = true
        };

        // destinatario = vos mismo
        mailMessage.To.Add(_settings.SenderEmail);

        // replyTo = mail del cliente
        if (!string.IsNullOrEmpty(replyTo))
            mailMessage.ReplyToList.Add(new MailAddress(replyTo));

        // Agregar la vista con logo embebido
        mailMessage.AlternateViews.Add(htmlView);

        using var client = new SmtpClient(_settings.SmtpServer, _settings.Port)
        {
            Credentials = new NetworkCredential(_settings.SenderEmail, _settings.Password),
            EnableSsl = _settings.EnableSsl
        };

        await client.SendMailAsync(mailMessage);
    }

    public void EnviarCorreoPedidoEnviado(string destinatario, string numeroPedido, string tracking = "")
    {
        try
        {
            var fromAddress = new MailAddress(_settings.SenderEmail, _settings.SenderName);
            var toAddress = new MailAddress(destinatario);

            string subject = $"Tu pedido #{numeroPedido} fue enviado 🚚";

            string trackingInfo = string.IsNullOrEmpty(tracking)
                ? ""
                : $"<p>Podés hacer el seguimiento con este número: <b>{tracking}</b></p>";

            string body = $@"
<html>
<head>
  <meta charset='UTF-8'>
  <style>
    body {{ font-family: Arial, sans-serif; background:#f5f5f5; }}
    .container {{ max-width:600px; margin:30px auto; background:#fff; border-radius:12px; padding:20px; box-shadow:0 4px 12px rgba(0,0,0,0.1); }}
    .header {{ background:#000; padding:20px; text-align:center; }}
    .header img {{ max-height:60px; }}
    .content {{ padding:20px; text-align:center; }}
    .btn {{ display:inline-block; margin-top:20px; padding:12px 24px; background:#000; color:#fff; text-decoration:none; border-radius:8px; font-weight:bold; }}
  </style>
</head>
<body>
  <div class='container'>
    <div class='header'>
      <img src='cid:LogoNM' alt='NM Relojes'>
    </div>
    <div class='content'>
      <h2>¡Tu pedido #{numeroPedido} ya está en camino!</h2>
      <p>Te avisamos que tu pedido fue despachado y pronto lo vas a recibir.</p>
      {trackingInfo}
      <a href='https://nmrelojes.com' class='btn'>Ver más productos</a>
    </div>
  </div>
</body>
</html>";

            // Crear la vista HTML con logo embebido
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
            var logoPath = Path.Combine(AppContext.BaseDirectory, "wwwroot/uploads/logo.jpeg");

            if (File.Exists(logoPath))
            {
                LinkedResource logo = new LinkedResource(logoPath);
                logo.ContentId = "LogoNM"; // debe coincidir con el cid del <img>
                htmlView.LinkedResources.Add(logo);
            }

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                IsBodyHtml = true
            })
            {
                message.AlternateViews.Add(htmlView);

                using (var smtp = new SmtpClient
                {
                    Host = _settings.SmtpServer,
                    Port = _settings.Port,
                    EnableSsl = _settings.EnableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_settings.SenderEmail, _settings.Password)
                })
                {
                    smtp.Send(message);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al enviar correo de pedido enviado: " + ex.Message);
        }
    }

    public void EnviarCorreoPedidoEntregado(string destinatario, string numeroPedido)
    {
        try
        {
            var fromAddress = new MailAddress(_settings.SenderEmail, _settings.SenderName);
            var toAddress = new MailAddress(destinatario);

            string subject = $"Pedido #{numeroPedido} entregado ✅";

            string body = $@"
<html>
<head>
  <meta charset='UTF-8'>
  <style>
    body {{ font-family: Arial, sans-serif; background:#f5f5f5; }}
    .container {{ max-width:600px; margin:30px auto; background:#fff; border-radius:12px; padding:20px; box-shadow:0 4px 12px rgba(0,0,0,0.1); }}
    .header {{ background:#000; padding:20px; text-align:center; }}
    .header img {{ max-height:60px; }}
    .content {{ padding:20px; text-align:center; }}
  </style>
</head>
<body>
  <div class='container'>
    <div class='header'>
      <img src='cid:LogoNM' alt='NM Relojes'>
    </div>
    <div class='content'>
      <h2>¡Pedido entregado!</h2>
      <p>Tu pedido <b>#{numeroPedido}</b> fue entregado con éxito.</p>
      <p>Esperamos que disfrutes tu compra ❤️</p>
      <p>Gracias por elegir <b>NM Relojes</b>.</p>
    </div>
  </div>
</body>
</html>";

            // Crear la vista HTML con logo embebido
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
            var logoPath = Path.Combine(AppContext.BaseDirectory, "wwwroot/uploads/logo.jpeg");

            if (File.Exists(logoPath))
            {
                LinkedResource logo = new LinkedResource(logoPath);
                logo.ContentId = "LogoNM"; // debe coincidir con el cid del <img>
                htmlView.LinkedResources.Add(logo);
            }

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                IsBodyHtml = true
            })
            {
                message.AlternateViews.Add(htmlView);

                using (var smtp = new SmtpClient
                {
                    Host = _settings.SmtpServer,
                    Port = _settings.Port,
                    EnableSsl = _settings.EnableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_settings.SenderEmail, _settings.Password)
                })
                {
                    smtp.Send(message);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al enviar correo de pedido entregado: " + ex.Message);
        }
    }

    public void EnviarCorreoArrepentimiento(string nombre, string telefono, string destinatarioCliente, string codigoCompra, string inconveniente)
    {
        try
        {
            var fromAddress = new MailAddress(_settings.SenderEmail, _settings.SenderName);
            var toAddress = new MailAddress(_settings.SenderEmail); // tu mail donde recibes la solicitud

            string subject = $"Solicitud de arrepentimiento de compra - {nombre}";

            string body = $@"
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: Arial, sans-serif; background:#f5f5f5; }}
        .container {{ max-width:600px; margin:30px auto; background:#fff; border-radius:12px; padding:20px; box-shadow:0 4px 12px rgba(0,0,0,0.1); }}
        .header {{ background:#000; padding:20px; text-align:center; }}
        .header img {{ max-height:60px; }}
        .content {{ padding:20px; }}
        .content p {{ line-height:1.5; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <img src='cid:LogoNM' alt='NM Relojes'>
        </div>
        <div class='content'>
            <h2>Solicitud de arrepentimiento de compra</h2>
            <p><b>Nombre:</b> {nombre}</p>
            <p><b>Teléfono:</b> {telefono}</p>
            <p><b>Email:</b> {destinatarioCliente}</p>
            <p><b>Código de compra:</b> {codigoCompra}</p>
            <p><b>Inconveniente:</b> {inconveniente}</p>
        </div>
    </div>
</body>
</html>";

            // Vista HTML con logo embebido
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
            var logoPath = Path.Combine(AppContext.BaseDirectory, "wwwroot/uploads/logo.jpeg");

            if (File.Exists(logoPath))
            {
                LinkedResource logo = new LinkedResource(logoPath);
                logo.ContentId = "LogoNM"; // debe coincidir con el cid del <img>
                htmlView.LinkedResources.Add(logo);
            }

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                IsBodyHtml = true
            })
            {
                message.AlternateViews.Add(htmlView);

                using (var smtp = new SmtpClient
                {
                    Host = _settings.SmtpServer,
                    Port = _settings.Port,
                    EnableSsl = _settings.EnableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_settings.SenderEmail, _settings.Password)
                })
                {
                    smtp.Send(message);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al enviar correo de arrepentimiento: " + ex.Message);
        }
    }


}




