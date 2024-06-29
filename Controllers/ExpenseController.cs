using ConstructApp.Constants;
using ConstructApp.Data;
using ConstructApp.Models;
using ConstructApp.Models.ViewModels;
using ConstructApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.ObjectModelRemoting;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Linq;

namespace ConstructApp.Controllers
{

    public class ExpenseController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly INotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment webHostEnvironment;



        public ExpenseController(ApplicationDbContext _dbContext, INotificationService _notification, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment environment)
        {
            dbContext = _dbContext;
            _notificationService = _notification;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            webHostEnvironment = environment;
        }
        public IActionResult Index()
        {
            var expenses = dbContext.Expenses
                .Include(e => e.ExpenseType)
                .Include(e => e.Project)
                .ToList();
            var viewModel = new ExpenseListVM
            {
                Expenses = expenses
            };
            return View(viewModel);
        }

        public IActionResult ExpenseGraph()
        {
            ExpenseVM expenseVM = new()
            {
                ProjectList = dbContext.Projects.Select(u => new SelectListItem
                {
                    Text = u.ProjectName,
                    Value = u.Id.ToString()
                }),
                Expense = new Expense(),
                ChartType = "weekly"
            };
            return View(expenseVM);
        }

        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            if (!user.CanRequestForSomeone)
            {
                TempData["error"] = "You are not authorized to create an expense.";
                return RedirectToAction("Index");
            }

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
                Expense = new Expense
                {
                    CreatedBy = $"{user.FirstName} {user.LastName}"
                }
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

                    if (TryValidateModel(expenseVM.Expense))
                    {
                        if (expenseVM.SupportiveDocument != null)
                        {
                            string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "supportive_documents");
                            if (!Directory.Exists(uploadsFolder))
                            {
                                Directory.CreateDirectory(uploadsFolder);
                            }
                            string uniqueFileName = Guid.NewGuid().ToString() +"_" + expenseVM.SupportiveDocument.FileName;
                            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                            //expenseVM.SupportiveDocument.CopyTo(new FileStream(filePath, FileMode.Create));
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                expenseVM.SupportiveDocument.CopyTo(fileStream);
                            }
                            expenseVM.Expense.SupportiveDocumentPath = uniqueFileName;
                        }
                        if (IsMaterialExpense(expenseVM.Expense.ExpenseTypeId))
                        {
                            CalculateExpenseAmount(expenseVM);
                        }

                        dbContext.Expenses.Add(expenseVM.Expense);
                    }
                    else
                    {
                        TempData["error"] = "Validation failed for one or more expenses. Please check the provided data.";
                        PopulateDropdowns(expenseVM);
                        return BadRequest(TempData["error"]);
                    }


                    dbContext.SaveChanges();
                    _notificationService.SendNotification(expenseVM.Expense.Id, _httpContextAccessor);
                    TempData["success"] = "Expenses added successfully";
                    return RedirectToAction("Index", "Expense");
                }
                else
                {
                    TempData["error"] = "Validation failed. Please check the provided data.";

                    return View(expenseVM);
                }
            }
            catch (DbUpdateException ex)
            {
                TempData["error"] = "An error occurred while saving to the database: " + ex.Message;
                return View(expenseVM);
            }
            catch (Exception ex)
            {
                TempData["error"] = "An unexpected error occurred: " + ex.Message;
                return View(expenseVM);
            }


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
            // You can add additional logic for calculating expense amount for non-material expenses here if needed
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

        private bool IsMaterialExpense(int expenseTypeId)
        {
            var materialExpenseTypeIds = dbContext.ExpenseTypes
                 .Where(e => e.Name == "Materials Costs")
                 .Select(e => e.Id)
                 .ToList();

            return materialExpenseTypeIds.Contains(expenseTypeId);
        }



        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest(new { success = false, message = "ID parameter is missing." });
                }

                var expense = dbContext.Expenses
                    .Include(e => e.Project)
                    .Include(e => e.ExpenseType)
                    .FirstOrDefault(e => e.Id == id);
                if (expense == null)
                {
                    return NotFound(new { success = false, message = $"Expense with ID {id} not found." });
                }


                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                var projectList = dbContext.Projects.Select(u => new SelectListItem
                {
                    Text = u.ProjectName,
                    Value = u.Id.ToString(),
                    Selected = u.Id == expense.ProjectId
                }).ToList();

                var expenseTypeList = dbContext.ExpenseTypes.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                    Selected = u.Id == expense.ExpenseTypeId
                }).ToList();

                var expenseVM = new ExpenseVM
                {
                    Expense = expense,
                    ProjectList = projectList,
                    ExpenseTypeList = expenseTypeList
                };

                return View(expenseVM);
            }
            catch (Exception ex)
            {
                TempData["error"] = "An error occurred: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(ExpenseVM expenseVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var expense = await dbContext.Expenses.FindAsync(expenseVM.Expense.Id);
                    if (expense == null) 
                    {
                        return NotFound();
                    }
                    expense.ProjectId = expenseVM.ProjectId;
                    expense.ExpenseTypeId = expenseVM.Expense.ExpenseTypeId;
                    expense.CreatedDate = expenseVM.Expense.CreatedDate;

                    if (expenseVM.SupportiveDocument != null && expenseVM.SupportiveDocument.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "supportive_documents");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + expenseVM.SupportiveDocument.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            expenseVM.SupportiveDocument.CopyTo(fileStream);
                        }
                        expenseVM.Expense.SupportiveDocumentPath = uniqueFileName;
                    }
                    dbContext.Update(expense);
                    await dbContext.SaveChangesAsync();

                }

                return View(expenseVM);
            }
            catch (Exception ex)
            {

                TempData["error"] = "An error occurred: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        public IActionResult AllRequests()
        {
            var allRequests = dbContext.Expenses.Where(e => e.ApprovalStatus == ApprovalStatus.Approved || e.ApprovalStatus == ApprovalStatus.Rejected)
             .Include(e => e.Project)
             .Include(e => e.ExpenseType)
             .Include(e => e.Approval)
             .ToList();

            return View(allRequests);  
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
                        e.CreatedBy,
                        Date = e.CreatedDate.ToString("MM/dd/yyyy"),
                        ApprovalStatus = e.ApprovalStatus.ToString()
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
                    return NotFound(new { success = false, message = "Expense not found." });
                }

                dbContext.Expenses.Remove(expense);
                dbContext.SaveChanges();

                return Json(new { success = true, message = "Expense deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while deleting the expense.", error = ex.Message });
            }
        }


        [HttpGet]
        public IActionResult GetPendingExpensesCount()
        {
            try
            {
                var pendingExpensesCount = dbContext.Expenses.Count(e => e.ApprovalStatus == ApprovalStatus.Pending);
                return Json(new { count = pendingExpensesCount });
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to fetch count of pending expenses: " + ex.Message);
            }
        }

        [HttpGet("Expense/ShowExpenseChart")]
        public async Task<IActionResult> ShowExpenseChart(int projectId, string chartType)
        {
            try
            {
                var project = await dbContext.Projects.FindAsync(projectId);
                if (project == null)
                {
                    return NotFound($"Project with ID {projectId} not found.");
                }

                switch (chartType)
                {
                    case "daily":
                        ViewBag.ChartType = "Daily";
                        break;
                    case "weekly":
                        ViewBag.ChartType = "Weekly";
                        break;
                    case "monthly":
                        ViewBag.ChartType = "Monthly";
                        break;
                    default:
                        throw new ArgumentException("Invalid chart type.");
                }

                DateTime startDate;
                switch (chartType)
                {
                    case "daily":
                        startDate = DateTime.Today.AddDays(-1);
                        break;
                    case "weekly":
                        startDate = DateTime.Today.AddDays(-7);
                        break;
                    case "monthly":
                        startDate = DateTime.Today.AddMonths(-1);
                        break;
                    default:
                        throw new ArgumentException("Invalid chart type.");
                }

                var expenses = await dbContext.Expenses
                    .Where(e => e.ProjectId == projectId && e.CreatedDate >= startDate && e.ApprovalStatus == ApprovalStatus.Approved)
                    .Include(e => e.ExpenseType)
                    .GroupBy(e => e.ExpenseType.Name)
                    .Select(g => new { ExpenseType = g.Key, Total = g.Sum(e => e.ExpenseAmount) })
                    .ToDictionaryAsync(g => g.ExpenseType, g => g.Total);

                return View("ExpenseChart", expenses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
    #endregion

}


