using Application.Interfaces;
using BCrypt.Net;

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
    }
}
