using Application.Model;
using Application.Model.Request;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IClientServices
    {
        Client Create (ClientCreateRequest clientCreateRequest);
        void Delete (int id);
        void Update (int id, ClientUpdatedRequest clientUpdatedRequest);
        ClientDto GetById (int id);
        List<ClientDto> GetAll ();
        List<Client> GetAllFullData ();
    }
}
