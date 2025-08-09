using Application.Models.Requests;

namespace Application.Interfaces
{
    public interface IClientService
    {
        int RegisterClient(ClientRegisterRequest request);
    }
}
