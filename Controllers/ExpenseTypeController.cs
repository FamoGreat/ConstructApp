using ConstructApp.Data;
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
                TempData["success"] = "Expense Type created successfully";

                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> EditExpenseType(int expenseTypeId, string expenseType)
        {
            ExpenseType existingExpenseType = await dbContext.ExpenseTypes.FindAsync(expenseTypeId);
            if (existingExpenseType != null)
            {
                existingExpenseType.Name = expenseType;
                TempData["success"] = "Expense Type Updated successfully";

                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteExpenseType(int expenseTypeId)
        {
            ExpenseType expenseTypeToDelete = await dbContext.ExpenseTypes.FindAsync(expenseTypeId);
            if (expenseTypeToDelete != null)
            {
                dbContext.ExpenseTypes.Remove(expenseTypeToDelete);
                TempData["success"] = "Expense TypeDeleted successfully";

                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
