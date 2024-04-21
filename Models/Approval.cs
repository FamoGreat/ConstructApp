using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConstructApp.Models
{
    public class Approval
    {
        public int Id { get; set; }

        public string? ApproverId { get; set; }
        public virtual ApplicationUser? Approver { get; set; }
        public int ExpenseId { get; set; }
        public virtual Expense? Expense { get; set; }

        [Required]
        public DateTime ApprovalDate { get; set; }

        [StringLength(100)]
        public string? Description { get; set; }

    }
}
