using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Model.Request
{
    public class CheckoutResponseDto
    {
        public string? PreferenceId { get; set; }
        public string? InitPoint { get; set; } // link para redirigir

        public string Id { get; set; } = string.Empty;   // preference.Id

        public string? SandboxInitPoint { get; set; }



    }
}
