using ConstructApp.Data;
using ConstructApp.Helpers;
using ConstructApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConstructApp.Controllers
{
    public class ExpenseTypeController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public ExpenseTypeController(ApplicationDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        public async Task<IActionResult> Index()
        {
            List<ExpenseType> expenseTypeList = await dbContext.ExpenseTypes.ToListAsync();
            return View(expenseTypeList);
        }

        [HttpPost]
        public async Task<IActionResult> CreateExpenseType(string expenseTypeName)
        {
            if (!string.IsNullOrEmpty(expenseTypeName))
            {
                ExpenseType newExpenseType = new ExpenseType { Name = expenseTypeName };
                dbContext.ExpenseTypes.Add(newExpenseType);

                await dbContext.SaveChangesAsync();

                ChangeTrackingHelper.LogChanges(null, newExpenseType, EntityState.Added, "Expense type created", dbContext, User.Identity.Name);

                TempData["success"] = "Expense Type created successfully";

            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> EditExpenseType(int expenseTypeId, string expenseType)
        {
            ExpenseType existingExpenseType = await dbContext.ExpenseTypes.FindAsync(expenseTypeId);
            if (existingExpenseType != null)
            {
                var originalExpenseType = new ExpenseType
                {
                    Id = existingExpenseType.Id,
                    Name = existingExpenseType.Name
                };

                existingExpenseType.Name = expenseType;

                await dbContext.SaveChangesAsync();

                ChangeTrackingHelper.LogChanges(originalExpenseType, existingExpenseType, EntityState.Modified, "Expense type updated", dbContext, User.Identity.Name);

                TempData["success"] = "Expense Type updated successfully";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteExpenseType(int expenseTypeId)
        {
            ExpenseType expenseTypeToDelete = await dbContext.ExpenseTypes.FindAsync(expenseTypeId);
            if (expenseTypeToDelete != null)
            {
                //var originalExpenseType = new ExpenseType
                //{
                //    Id = expenseTypeToDelete.Id,
                //    Name = expenseTypeToDelete.Name
                //};

                dbContext.ExpenseTypes.Remove(expenseTypeToDelete);
                await dbContext.SaveChangesAsync();

                //ChangeTrackingHelper.LogChanges(originalExpenseType, null, EntityState.Deleted, "Expense type deleted", dbContext, User.Identity.Name);

                TempData["success"] = "Expense Type deleted successfully";
            }
            return RedirectToAction("Index");
        }

    }
}
