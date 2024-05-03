using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ConstructApp.Models.ViewModels
{
    public class RegisterVM
    {
            public string? Id { get; set; }

            [Required(ErrorMessage = "Email is required")]
            [MaxLength(256, ErrorMessage = "Email must be at most 256 characters")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            public string? Email { get; set; }

            [Required(ErrorMessage = "Phone number is required")]
            [MaxLength(256, ErrorMessage = "Phone number must be at most 256 characters")]
            [RegularExpression(@"^(\+\d{1,3}[- ]?)?\d{10}$", ErrorMessage = "Invalid phone number")]
            public string? PhoneNumber { get; set; }

            //[Required]
            //[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string? Password { get; set; }
            //[DataType(DataType.Password)]
            //[Display(Name = "Confirm password")]
            //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string? ConfirmPassword { get; set; }

            [Required(ErrorMessage = "First Name is required")]
            public string? FirstName { get; set; }
            [Required(ErrorMessage = "Last Name is required")]
            public string? LastName { get; set; }

            public byte[]? Signature { get; set; }
            public IFormFile? SignatureFile { get; set; }

            [Required(ErrorMessage = "Please select a role")]
            [Display(Name = "Role")]
            public string? Role { get; set; }

            public SelectList? Roles { get; set; }
            public bool CanApproved { get; set; }
            public bool CanRequestForSomeone { get; set; }

        }
    }

