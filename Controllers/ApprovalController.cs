using ConstructApp.Data;
using ConstructApp.Helpers;
using ConstructApp.Models;
using ConstructApp.Models.ViewModels;
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
        public IActionResult PendingRequest()
        {
            var expenses = dbContext.Expenses
                           .Where(e => e.ApprovalStatus == ApprovalStatus.Pending)
                           .Include(e => e.Project)
                           .Include(e => e.ExpenseType)
                           .ToList();

            var expenseListVM = new ExpenseListVM
            {
                Expenses = expenses
            };
            return View(expenseListVM);
        }
        [HttpPost]
        public IActionResult Approve(int expenseId, string approvalStatus, string description)
        {
            var expense = dbContext.Expenses.Include(e => e.Approval).FirstOrDefault(e => e.Id == expenseId);
            if (expense == null)
            {
                return NotFound();
            }

            var oldApproval = expense.Approval != null ? new Approval
            {
                ApprovalDate = expense.Approval.ApprovalDate,
                Description = expense.Approval.Description
            } : null;

            if (approvalStatus == "Approved")
            {
                expense.ApprovalStatus = ApprovalStatus.Approved;
                if (expense.Approval != null)
                {
                    expense.Approval.Description = description;
                }
                else
                {
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
                if (expense.Approval != null)
                {
                    expense.Approval.Description = description;
                }
                else
                {
                    expense.Approval = new Approval
                    {
                        ApprovalDate = DateTime.Now,
                        Description = description
                    };
                }
            }

            dbContext.SaveChanges();

            ChangeTrackingHelper.LogChanges<Approval>(oldApproval, expense.Approval, EntityState.Modified, "Approval updated", dbContext, User.Identity.Name);

            return RedirectToAction(nameof(PendingRequest));
        }

        public IActionResult ApprovedRequest()
        {
            var approved = dbContext.Expenses.Where(e => e.ApprovalStatus == ApprovalStatus.Approved || e.ApprovalStatus == ApprovalStatus.Rejected)
                .Include(e => e.Project)
                .Include(e => e.ExpenseType)
                .Include(e => e.Approval)
                .ToList();

            return View(approved);
        }
    }

}
