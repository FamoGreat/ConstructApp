using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConstructApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string? Name { get; set; }
        public string? ProfileImage { get; set; }
        public string? Signature { get; set; }
        [NotMapped]
        public string? Role { get; set; }
    }
}
