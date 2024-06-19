using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Model.Request
{
    public class SuperAdminCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Password {  get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }   
}
