namespace Application.Models.Requests
{
    public class ClientRegisterRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string? Name { get; set; }
        public string? LastName { get; set; }
    }
}
