using ConstructApp.Data;
using ConstructApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConstructApp.Controllers
{

    public class ApprovalController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        public ApprovalController(ApplicationDbContext _dbContext)
        {
            dbContext = _dbContext;
        }
        public IActionResult Index()
        {
            var expenses = dbContext.Expenses
                           .Where(e => e.ApprovalStatus == ApprovalStatus.Pending)
                           .Include(e => e.Project)
                           .Include(e => e.ExpenseType)
                           .ToList();
            return View(expenses);
        }

        [HttpPost]
        public IActionResult Approve(int expenseId, string approvalStatus, string description)
        {
            var expense = dbContext.Expenses.Include(e => e.Approval).FirstOrDefault(e => e.Id == expenseId);
            if (expense == null)
            {
                return NotFound();
            }

            // Update approval status and description
            if (approvalStatus == "Approved")
            {
                expense.ApprovalStatus = ApprovalStatus.Approved;
                // If Approval record exists, update its description
                if (expense.Approval != null)
                {
                    expense.Approval.Description = description;
                }
                else
                {
                    // If no Approval record exists, create a new one
                    expense.Approval = new Approval
                    {
                        ApprovalDate = DateTime.Now,
                        Description = description
                    };
                }
            }
            else if (approvalStatus == "Rejected")
            {
                expense.ApprovalStatus = ApprovalStatus.Rejected;
                // If Approval record exists, update its description
                if (expense.Approval != null)
                {
                    expense.Approval.Description = description;
                }
                else
                {
                    // If no Approval record exists, create a new one
                    expense.Approval = new Approval
                    {
                        ApprovalDate = DateTime.Now,
                        Description = description
                    };
                }
            }

            dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }



    }

}
