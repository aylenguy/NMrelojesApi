using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Model
{
    public class AdminDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public static AdminDto Create(Admin admin) 
        {
            var dto = new AdminDto();
            dto.Id = admin.Id;
            dto.Name = admin.Name;
            dto.LastName = admin.LastName;

            return dto;
        }

        public static List<AdminDto> CreateList(IEnumerable<Admin> admin)
        {
            List<AdminDto> listDto = [];
            foreach (var a in admin)
            {
                listDto.Add(Create(a));
            }

            return listDto;
        }
    }
}
