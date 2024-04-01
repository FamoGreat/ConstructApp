using System.ComponentModel.DataAnnotations;

namespace ConstructApp.Models
{
    public class ExpenseType
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
    }
}
