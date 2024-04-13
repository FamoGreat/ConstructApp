using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConstructApp.Models
{
    public enum ApprovalStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class Approval
    {
        public int Id { get; set; }

        public int ExpenseId { get; set; }
        public virtual Expense? Expense { get; set; }

        public int ProjectId { get; set; }
        public virtual Project? Project { get; set; }

        public string? ApproverId { get; set; }
        public virtual ApplicationUser? Approver { get; set; }

        [Required]
        public DateTime ApprovalDate { get; set; }

        [StringLength(100)]
        public string? Description { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }

    }
}
