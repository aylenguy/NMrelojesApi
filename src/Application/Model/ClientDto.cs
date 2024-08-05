using Domain.Entities;
using Domain.Entities.Domain.Entities;
using System.Collections.Generic;

namespace Application.Model
{
    public class ClientDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public static ClientDto Create(Client client)
        {
            return new ClientDto
            {
                Id = client.Id,
                Name = client.Name,
                LastName = client.LastName
            };
        }

        public static List<ClientDto> CreateList(IEnumerable<Client> clients)
        {
            var listDto = new List<ClientDto>();
            foreach (var client in clients)
            {
                listDto.Add(Create(client));
            }
            return listDto;
        }
    }
}

