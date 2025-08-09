using Application.Models.Requests;

namespace Application.Interfaces
{
    public interface ICustomAuthenticationService
    {
        string Authenticate(CredentialsDtoRequest credentialsRequest);
        string AuthenticateAdmin(CredentialsDtoRequest credentialsRequest);
    }
}
