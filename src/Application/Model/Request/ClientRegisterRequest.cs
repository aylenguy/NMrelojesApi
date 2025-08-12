namespace Application.Models.Requests
{
    public class ClientRegisterRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string UserName { get; set; } = null!;
        public string LastName { get; set; } = null!;
    }
}
