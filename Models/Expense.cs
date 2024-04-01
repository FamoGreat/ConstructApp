using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConstructApp.Models
{
    public class Expense
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("Project")]
        public int ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        [ValidateNever]
        public Project? Project { get; set; }
        [Required]
        [DisplayName("Expense Type")]
        public int ExpenseTypeId { get; set; }
        [ForeignKey("ExpenseTypeId")]
        [ValidateNever]
        public ExpenseType? ExpenseType { get; set; }
        [Required]
        [DisplayName("Expense Amount")]
        public decimal ExpenseAmount { get; set; }
        [Required]
        [DisplayName("Created By")]
        public string? CreatedBy { get; set; }
        [Required]
        [DisplayName("Created Date")]
        public DateTime CreatedDate { get; set; }

    }
}
