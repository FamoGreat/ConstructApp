using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConstructApp.Models
{
    public class Expense
    {
        public int Id { get; set; }
        [DisplayName("Project")]
        public int ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        [ValidateNever]
        public Project? Project { get; set; }
        [DisplayName("Expense Type")]
        public int ExpenseTypeId { get; set; }
        [ForeignKey("ExpenseTypeId")]
        [ValidateNever]
        public ExpenseType? ExpenseType { get; set; }
        [DisplayName("Expense Amount")]
        public decimal ExpenseAmount { get; set; }
        [DisplayName("Created By")]
        [Required]
        public string? CreatedBy { get; set; }
        [DisplayName("Created Date")]
        [Required]
        public DateTime CreatedDate { get; set; }

    }
}
