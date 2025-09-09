using Application.Interfaces;
using BCrypt.Net;
using System.Text.RegularExpressions;

namespace Infrastructure.Data.Services
{
    public class PasswordService : IPasswordService
    {
        private const int WorkFactor = 12; // Puedes ajustar el coste

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: WorkFactor);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        public bool IsValid(string password)
        {
            // ✅ Min 8 caracteres, 1 mayúscula, 1 minúscula, 1 número
            var regex = new Regex(@".{8,}");

            return regex.IsMatch(password);
        }
    }
}
