using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;
using Microsoft.AspNetCore.Components.Routing;

namespace Domain.Entities
{
    public abstract class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Required]
        public string? Name { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Required]
        public required string LastName { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [EmailAddress]
        [Required]
        public required string Email { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Required]
        public required string Password { get; set; }

        [Required]
        public RolUser Rol { get; set; }
    }
}
