
using System.ComponentModel.DataAnnotations;

namespace ConstructApp.Models
{
    internal class AllowedFileExtensionsAttribute : ValidationAttribute
    {
       private readonly string [] _extensions;
        public AllowedFileExtensionsAttribute(params string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            IFormFile file = value as IFormFile;
            if (file == null)
            {
                return new ValidationResult("The provided value is not a valid file.");
            }
            string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!string.IsNullOrEmpty(fileExtension))
            {
                return new ValidationResult($"Only {string.Join(", ", _extensions)} files are allowed.");
            }

            return ValidationResult.Success;  
        }
    }
}