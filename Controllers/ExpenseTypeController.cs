using ConstructApp.Data;
using ConstructApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConstructApp.Controllers
{

    public class ExpenseTypeController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        public ExpenseTypeController(ApplicationDbContext _dbContext)
        {
            dbContext = _dbContext;
        }
        public IActionResult Index()
        {
            List<ExpenseType> expenseTypeList = dbContext.ExpenseTypes.ToList();
            return View(expenseTypeList);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(ExpenseType expenseType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dbContext.ExpenseTypes.Add(expenseType);
                    dbContext.SaveChanges();
                    TempData["success"] = "Expense type added successfully";

                    return RedirectToAction("Index", "ExpenseType");
                }
                return View();
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred while adding the expense type: {ex.Message}";
                return View();
            }
        }
        public IActionResult Edit(int? id)
        {
            try
            {
                if (id == null || id == 0)
                {
                    return NotFound();
                }

                var expenseType = dbContext.ExpenseTypes.FirstOrDefault(e => e.Id == id);
                if (expenseType == null)
                {
                    return NotFound();
                }
                return View(expenseType);
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred while retrieving the expense type: {ex.Message}";
                return RedirectToAction("Index", "ExpenseType");
            }
        }

        [HttpPost]
        public IActionResult Edit(ExpenseType expenseType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dbContext.Update(expenseType);
                    dbContext.SaveChanges();
                    TempData["success"] = "Expense Type updated successfully";

                    return RedirectToAction("Index", "ExpenseType");
                }
                return View(expenseType);
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred while updating the expense type: {ex.Message}";
                return View(expenseType);
            }
        }




        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                List<ExpenseType> expenseTypes = dbContext.ExpenseTypes.ToList();
                return Json(new { success = true, data = expenseTypes });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred while retrieving expense types: {ex.Message}" });
            }
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return Json(new { success = false, message = "Invalid expense type ID" });
                }

                var expenseTypeToBeDeleted = dbContext.ExpenseTypes.FirstOrDefault(e => e.Id == id);
                if (expenseTypeToBeDeleted == null)
                {
                    return Json(new { success = false, message = "Expense type not found" });
                }

                dbContext.Remove(expenseTypeToBeDeleted);
                dbContext.SaveChanges();
                return Json(new { success = true, message = "Expense type deleted successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred while deleting the expense type: {ex.Message}" });
            }
        }
        #endregion

    }
}
