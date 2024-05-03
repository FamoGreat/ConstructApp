using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConstructApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public byte[]? ProfileImage { get; set; }
        public byte[]? Signature { get; set; }
        public bool IsApproved { get; set; }
        public bool CanApproved { get; set; }
        public bool CanRequestForSomeone { get; set; }

    }
}
