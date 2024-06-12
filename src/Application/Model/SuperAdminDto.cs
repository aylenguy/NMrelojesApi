using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Model
{
    public class SuperAdminDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public static SuperAdminDto Create(SuperAdmin superAdmin) 
        {
            var dto = new SuperAdminDto();
            dto.Id = superAdmin.Id;
            dto.Name = superAdmin.Name;
            dto.LastName = superAdmin.LastName;

            return dto;
        }

        public static List<SuperAdminDto> CreateList(IEnumerable<SuperAdmin> superAdmins)
        {
            List<SuperAdminDto> listDto = [];
            foreach (var s in superAdmins) 
            {
                listDto.Add(Create(s));
            }

            return listDto;

        }
    }

}
