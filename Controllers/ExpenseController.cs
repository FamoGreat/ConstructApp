using ConstructApp.Data;
using ConstructApp.Models;
using ConstructApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.ObjectModelRemoting;
using Microsoft.EntityFrameworkCore;

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
            List<Expense> expenseList = dbContext.Expenses.ToList();
           
            return View(expenseList);
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

            if (ModelState.IsValid)
            {
                dbContext.Expenses.Add(expenseVM.Expense);
                dbContext.SaveChanges();
                TempData["success"] = "Expense added successfully";

                return RedirectToAction("Index", "Expense");
            }
            else
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
                return View(expenseVM);
            }
        }

        #region API CALLS
        [HttpGet]
        public IActionResult CalculateMaterialCost(int projectId, int expenseType)
        {
            // Your logic to calculate the total material cost based on the selected project and expense type
            decimal totalMaterialCost = (decimal)CalculateTotalMaterialCost(projectId);

            // Return the total material cost as JSON
            return Json(new { totalMaterialCost = totalMaterialCost });
        }

        public double CalculateTotalMaterialCost(int projectId)
        {
            // Retrieve project materials associated with the project from the database
            var projectMaterials = dbContext.ProjectMaterials.Where(pm => pm.ProjectId == projectId).ToList();

            double totalMaterialCost = 0;

            // Sum up the total cost of all project materials
            foreach (var material in projectMaterials)
            {
                // Assuming ProjectMaterial has properties MaterialCost and MaterialQuantity
                double materialCost = material.EstimatedQuantity;
                double materialQuantity = material.EstimatedQuantity;

                // Calculate total cost for this material and add it to the overall cost
                totalMaterialCost += materialCost * materialQuantity;
            }

            return totalMaterialCost;
        }
         #endregion

    }
}
