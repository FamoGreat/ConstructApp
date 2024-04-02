using ConstructApp.Data;
using ConstructApp.Models;
using ConstructApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.ObjectModelRemoting;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ConstructApp.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        public ExpenseController(ApplicationDbContext _dbContext)
        {
            dbContext = _dbContext;
        }
        public IActionResult Index()
        {
            ExpenseVM expenseVM = new()
            {
                ProjectList = dbContext.Projects.Select(u => new SelectListItem
                {
                    Text = u.ProjectName,
                    Value = u.Id.ToString()
                })
            };



            return View(expenseVM);

        }
      
        public IActionResult Create()
        {
            ExpenseVM expenseVM = new()
            {
                ProjectList = dbContext.Projects.Select(u => new SelectListItem
                {
                    Text = u.ProjectName,
                    Value = u.Id.ToString()
                }),
                ExpenseTypeList = dbContext.ExpenseTypes.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Expense = new Expense()
            };


            return View(expenseVM);
        }
        [HttpPost]
        public IActionResult Create(ExpenseVM expenseVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (isMaterialExpense(expenseVM.Expense.ExpenseTypeId))
                    {
                        CalculateExpenseAmount(expenseVM);
                    }
                    dbContext.Expenses.Add(expenseVM.Expense);
                    dbContext.SaveChanges();
                    TempData["success"] = "Expense added successfully";
                    return RedirectToAction("Index", "Expense");
                }
            }
            catch (DbUpdateException ex)
            {
                TempData["error"] = "An error occurred while saving to the database: " + ex.Message;
            }
            catch (Exception ex)
            {
                TempData["error"] = "An unexpected error occurred: " + ex.Message;
            }

            PopulateDropdowns(expenseVM);
            return View(expenseVM);
        }

        private void CalculateExpenseAmount(ExpenseVM expenseVM)
        {
            if (expenseVM.Expense.ProjectId != 0)
            {
                var projectId = expenseVM.Expense.ProjectId;
                var projectMaterials = dbContext.ProjectMaterials
                    .Where(pm => pm.ProjectId == projectId)
                    .ToList();
                decimal totalMaterialCost = projectMaterials.Sum(pm => pm.EstimatedCost * pm.EstimatedQuantity);
                expenseVM.Expense.ExpenseAmount = totalMaterialCost;
            }
        }

        private void PopulateDropdowns(ExpenseVM expenseVM)
        {
            expenseVM.ProjectList = dbContext.Projects.Select(u => new SelectListItem
            {
                Text = u.ProjectName,
                Value = u.Id.ToString()
            });

            expenseVM.ExpenseTypeList = dbContext.ExpenseTypes.Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
        }

        private bool isMaterialExpense(int expenseTypeId)
        {
            var materialExpenseTypeIds = dbContext.ExpenseTypes
                 .Where(e => e.Name == "Material") 
                 .Select(e => e.Id)
                 .ToList();

            return materialExpenseTypeIds.Contains(expenseTypeId);
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetExpenses(int projectId)
        {
            try
            {
                var expenses = dbContext.Expenses
                    .Where(e => e.ProjectId == projectId)
                    .Select(e => new
                    {
                        e.Id,
                        ExpenseType = e.ExpenseType.Name,
                        Amount = e.ExpenseAmount,
                        Date = e.CreatedDate.ToString("MM/dd/yyyy")
                    })
                    .ToList();

                return Json(expenses);
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to fetch expenses: " + ex.Message);
            }
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            try
            {
                var expense = dbContext.Expenses.Find(id);
                if (expense == null)
                {
                    return NotFound();
                }

                dbContext.Expenses.Remove(expense);
                dbContext.SaveChanges();

                return Json(new { success = true, message = "Expense deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while deleting the expense: " + ex.Message });
            }
        }

        #endregion

    }



}

