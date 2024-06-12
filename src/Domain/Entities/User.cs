using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public abstract class User
    {
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string Name{ get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string LastName { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        [EmailAddress]
        public string Email { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string Password {  get; set; } 
        

    }
}
