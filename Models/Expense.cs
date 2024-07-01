using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConstructApp.Models
{
    public class Expense
    {
        public int Id { get; set; }
        [ForeignKey("ProjectId")]
        [ValidateNever]
        [DisplayName("Project")]
        public int ProjectId { get; set; }
        public Project? Project { get; set; }
        [DisplayName("Expense Type")]
        public ExpenseType? ExpenseType { get; set; }

        [ForeignKey("ExpenseTypeId")]
        [ValidateNever]
        public int ExpenseTypeId { get; set; }
        [DisplayName("Expense Amount")]
        public decimal ExpenseAmount { get; set; }
        [DisplayName("Created By")]
        [Required]
        public string? CreatedBy { get; set; }
        [DisplayName("Created Date")]
        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Today;
        public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;
        [ValidateNever]
        public  virtual Approval Approval { get; set; }

        public string? SupportiveDocumentPath { get; set; }

        public string? Description { get; set; }

        public static implicit operator Expense(List<Expense> v)
        {
            throw new NotImplementedException();
        }
    }

    public enum ApprovalStatus
    {
        Pending,
        Approved,
        Rejected
    }
}
