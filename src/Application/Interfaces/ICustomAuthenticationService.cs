using Application.Model;
using Application.Models.Requests;

namespace Application.Interfaces
{
    public interface ICustomAuthenticationService
    {
        AuthResult Authenticate(CredentialsDtoRequest credentialsRequest);
        AuthResult AuthenticateAdmin(CredentialsDtoRequest credentialsRequest);
    }
}
