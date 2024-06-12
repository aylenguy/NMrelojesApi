using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Model
{
    public class ClientDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public static ClientDto Create(Client client)
        {
            var dto = new ClientDto();
            dto.Id = client.Id;
            dto.Name = client.Name;
            dto.LastName = client.LastName;

            return dto;
        }

        public static List<ClientDto> CreateList(IEnumerable<Client> client)
        {
            List<ClientDto> listDto = [];
            foreach (var c in client)
            {
                listDto.Add(Create(c));
            }

            return listDto;

        }
    }

}

