using ConstructApp.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Approval
{
    public int Id { get; set; }

    // Modify this property to ensure it references the correct type and name of the foreign key
    [ForeignKey("ApproverId")]
    public string? ApproverId { get; set; }
    public virtual ApplicationUser? Approver { get; set; }
    [ForeignKey("ExpenseId")]
    public int ExpenseId { get; set; }
    public virtual Expense? Expense { get; set; }

    [Required]
    public DateTime ApprovalDate { get; set; } = DateTime.Today;

    [StringLength(100)]
    public string? Description { get; set; }
}