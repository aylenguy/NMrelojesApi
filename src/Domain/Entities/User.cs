public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string UserType { get; set; } = null!;
    public string? UserName { get; set; }
    public string? Name { get; set; }
    public string? LastName { get; set; }

    // Si es un hash real debería ser byte[], pero lo dejo string si tu implementación es así
    public string PasswordHash { get; set; } = string.Empty;

    // Si guardas la contraseña en texto plano (mala práctica) se queda aquí
   
}

