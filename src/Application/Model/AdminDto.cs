using Domain.Entities;
using System.Collections.Generic;

namespace Application.Model
{
    public class AdminDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public static AdminDto Create(Admin admin)
        {
            return new AdminDto
            {
                Id = admin.Id,
                Name = admin.Name,
                LastName = admin.LastName
            };
        }

        public static List<AdminDto> CreateList(IEnumerable<Admin> admins)
        {
            var listDto = new List<AdminDto>();
            foreach (var admin in admins)
            {
                listDto.Add(Create(admin));
            }
            return listDto;
        }
    }
}
