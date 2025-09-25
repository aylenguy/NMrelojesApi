using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Model.Request
{
    public class CheckoutResponseDto
    {
        public string Id { get; set; } = string.Empty;             // preference.Id de MercadoPago
        public string? InitPoint { get; set; }                     // link para redirigir al checkout
        public string? SandboxInitPoint { get; set; }             // link sandbox (opcional)
        public string ExternalReference { get; set; } = string.Empty; // tu externalReference de la venta
    }
}
