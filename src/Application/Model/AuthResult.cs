using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Model
{
    public class AuthResult
    {
        public string? Token { get; set; }
        public string? UserType { get; set; }
        public string? Error { get; set; } // ej: "invalid_email", "user_not_found"
        public bool Success => string.IsNullOrEmpty(Error);
    }
}
