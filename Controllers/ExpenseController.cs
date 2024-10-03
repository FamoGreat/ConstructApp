using ConstructApp.Constants;
using ConstructApp.Data;
using ConstructApp.Helpers;
using ConstructApp.Models;
using ConstructApp.Models.ViewModels;
using ConstructApp.Services;
using Glimpse.Core.Extensibility;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment webHostEnvironment;



        public ExpenseController(ApplicationDbContext _dbContext,  UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment environment)
        {
            dbContext = _dbContext;
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

        [HttpGet]
        public IActionResult GetTodayExpenses()
        {
            var today = DateTime.Today;
            var todayExpenses = dbContext.Expenses
                .Where(e => e.CreatedDate.Date == today)
                .Sum(e => e.ExpenseAmount);

            return Json(new { totalTodayExpenses = todayExpenses });
        }
        [HttpGet]
        public IActionResult GetTotalExpenses()
        {
            var totalExpenses = dbContext.Expenses.Sum(e => e.ExpenseAmount);
            return Json(new { totalExpenses = totalExpenses });
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
                    if (expenseVM.SupportiveDocument != null)
                    {
                        // Handle supportive document upload
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

                    // Calculate expense amount based on expense type
                    if (IsMaterialExpense(expenseVM.Expense.ExpenseTypeId))
                    {
                        CalculateMaterialExpenseAmount(expenseVM);
                    }
                    else if (IsEquipmentExpense(expenseVM.Expense.ExpenseTypeId))
                    {
                        CalculateEquipmentExpenseAmount(expenseVM);
                    }

                    // Add expense to DbContext
                    dbContext.Expenses.Add(expenseVM.Expense);
                    dbContext.SaveChanges();

                    ChangeTrackingHelper.LogChanges<Expense>(null, expenseVM.Expense, EntityState.Added, "Expense created", dbContext, User.Identity.Name);

                    TempData["success"] = "Expense added successfully";
                    return RedirectToAction("Index", "Expense");
                }
                else
                {
                    TempData["error"] = "Validation failed. Please check the provided data.";
                    PopulateDropdowns(expenseVM); 
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


        //private void UpdateTotalExpenses(int projectId)
        //{
        //    var project = dbContext.Projects.FirstOrDefault(p => p.Id == projectId);
        //    if (project != null)
        //    {
        //        // Calculate total material cost
        //        decimal totalMaterialCost = dbContext.ProjectMaterials
        //            .Where(pm => pm.ProjectId == projectId)
        //            .Sum(pm => pm.EstimatedCost * pm.EstimatedQuantity);

        //        // Calculate total tool cost
        //        decimal totalToolCost = dbContext.ProjectTools
        //            .Where(pt => pt.ProjectId == projectId)
        //            .Sum(pt => pt.ToolCost * pt.ToolsQuantity);

        //        // Update project's total expenses
        //        project.TotalMaterialExpense = totalMaterialCost;
        //        project.TotalToolExpense = totalToolCost;

        //        // Save changes to the database
        //        dbContext.SaveChanges();
        //    }
        //}

        private void CalculateMaterialExpenseAmount(ExpenseVM expenseVM)
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

        private void CalculateEquipmentExpenseAmount(ExpenseVM expenseVM)
        {
            if (expenseVM.Expense.ProjectId != 0)
            {
                var projectId = expenseVM.Expense.ProjectId;
                var projectTools = dbContext.ProjectTools
                    .Where(pt => pt.ProjectId == projectId)
                    .ToList();
                decimal totalToolCost = projectTools.Sum(pt => pt.ToolCost * pt.ToolsQuantity);

                expenseVM.Expense.ExpenseAmount = totalToolCost;
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

        private bool IsMaterialExpense(int expenseTypeId)
        {
            var materialExpenseTypeIds = dbContext.ExpenseTypes
                 .Where(e => e.Name == "Materials Costs")
                 .Select(e => e.Id)
                 .ToList();

            return materialExpenseTypeIds.Contains(expenseTypeId);
        }

        private bool IsEquipmentExpense(int expenseTypeId)
        {
            var materialExpenseTypeIds = dbContext.ExpenseTypes
                 .Where(e => e.Name == "Equipment Costs")
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
        public IActionResult Edit(ExpenseVM expenseVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingExpense = dbContext.Expenses.FirstOrDefault(e => e.Id == expenseVM.Expense.Id);
                    if (existingExpense == null)
                    {
                        return NotFound();
                    }

                    var originalExpense = new Expense
                    {
                        ProjectId = existingExpense.ProjectId,
                        ExpenseTypeId = existingExpense.ExpenseTypeId,
                        CreatedBy = existingExpense.CreatedBy,
                        CreatedDate = existingExpense.CreatedDate,
                        Description = existingExpense.Description,
                        ApprovalStatus = existingExpense.ApprovalStatus,
                        ExpenseAmount = existingExpense.ExpenseAmount,
                        SupportiveDocumentPath = existingExpense.SupportiveDocumentPath
                    };
                    if (TryValidateModel(expenseVM.Expense))
                    {
                        if (expenseVM.SupportiveDocument != null)
                        {
                            if (!string.IsNullOrEmpty(existingExpense.SupportiveDocumentPath))
                            {
                                string oldFilePath = Path.Combine(webHostEnvironment.WebRootPath, "supportive_documents", existingExpense.SupportiveDocumentPath);
                                if (System.IO.File.Exists(oldFilePath))
                                {
                                    System.IO.File.Delete(oldFilePath);
                                }
                            }

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
                            existingExpense.SupportiveDocumentPath = uniqueFileName;
                        }

                        // Update basic expense properties
                        existingExpense.ProjectId = expenseVM.Expense.ProjectId;
                        existingExpense.ExpenseTypeId = expenseVM.Expense.ExpenseTypeId;
                        existingExpense.CreatedBy = expenseVM.Expense.CreatedBy;
                        existingExpense.CreatedDate = expenseVM.Expense.CreatedDate;
                        existingExpense.Description = expenseVM.Expense.Description;
                        existingExpense.ApprovalStatus = expenseVM.Expense.ApprovalStatus;

                        // Calculate expense amount based on expense type
                        CalculateExpenseAmount(existingExpense);

                        dbContext.SaveChanges();

                        ChangeTrackingHelper.LogChanges(originalExpense, existingExpense, EntityState.Modified, "Expense updated", dbContext, User.Identity.Name);

                        TempData["success"] = "Expense request updated successfully";
                        return RedirectToAction("Index", "Expense");
                    }
                    else
                    {
                        TempData["error"] = "Validation failed for one or more expenses. Please check the provided data.";
                        PopulateDropdowns(expenseVM);
                        return View(expenseVM);
                    }
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
        private void CalculateExpenseAmount(Expense expense)
        {
            if (IsMaterialExpense(expense.ExpenseTypeId))
            {
                var projectId = expense.ProjectId;
                var projectMaterials = dbContext.ProjectMaterials
                    .Where(pm => pm.ProjectId == projectId)
                    .ToList();
                decimal totalMaterialCost = projectMaterials.Sum(pm => pm.EstimatedCost * pm.EstimatedQuantity);
                expense.ExpenseAmount = totalMaterialCost;
            }
            else if (IsEquipmentExpense(expense.ExpenseTypeId))
            {
                var projectId = expense.ProjectId;
                var projectTools = dbContext.ProjectTools
                    .Where(pt => pt.ProjectId == projectId)
                    .ToList();
                decimal totalToolCost = projectTools.Sum(pt => pt.ToolCost * pt.ToolsQuantity);
                expense.ExpenseAmount = totalToolCost;
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

                // Capture the state of the entity before deletion
                //var originalExpense = new Expense
                //{
                //    Id = expense.Id,
                //    ProjectId = expense.ProjectId,
                //    ExpenseTypeId = expense.ExpenseTypeId,
                //    ExpenseAmount = expense.ExpenseAmount,
                //    CreatedBy = expense.CreatedBy,
                //    CreatedDate = expense.CreatedDate,
                //    ApprovalStatus = expense.ApprovalStatus,
                //    Description = expense.Description,
                //    SupportiveDocumentPath = expense.SupportiveDocumentPath
                //};

           

                dbContext.Expenses.Remove(expense);
                dbContext.SaveChanges();

                // Log the deletion
                //ChangeTrackingHelper.LogChanges(originalExpense, null, EntityState.Deleted, "Expense deleted", dbContext, User.Identity.Name);

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

        // GET: Expense/GetExpenseTypePercentages
        [HttpGet("Expense/GetExpenseTypePercentages")]
        public IActionResult GetExpenseTypePercentages()
        {
            var totalExpenses = dbContext.Expenses.Sum(e => e.ExpenseAmount);
            if (totalExpenses == 0)
            {
                return Ok(new List<ExpensePercentageViewModel>());
            }

            var expensePercentages = dbContext.Expenses
                .GroupBy(e => e.ExpenseType.Name)
                .Select(group => new ExpensePercentageViewModel
                {
                    ExpenseTypeName = group.Key,
                    Percentage = (group.Sum(e => e.ExpenseAmount) / totalExpenses) * 100
                })
                .ToList();

            return Ok(expensePercentages);
        }

        //[HttpGet("Expense/GetMonthlyExpenseReport")]
        //public IActionResult GetMonthlyExpenseReport()
        //{
        //    var monthlyExpenses = dbContext.Expenses
        //        .GroupBy(e => new { e.CreatedDate.Year, e.CreatedDate.Month })
        //        .Select(g => new
        //        {
        //            Month = g.Key.Month,
        //            Year = g.Key.Year,
        //            TotalExpense = g.Sum(e => e.ExpenseAmount)
        //        })
        //        .ToList();

        //    return Ok(monthlyExpenses);
        //}


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
                DateTime endDate = DateTime.Today;

                switch (chartType)
                {
                    case "daily":
                        startDate = DateTime.Today.AddDays(-1);
                        break;
                    case "weekly":
                        startDate = DateTime.Today.AddDays(-7);
                        break;
                    case "monthly":
                        startDate = DateTime.Today.AddMonths(-12);
                        break;
                    default:
                        throw new ArgumentException("Invalid chart type.");
                }

                // Fetch the raw expenses data
                var rawExpenses = await dbContext.Expenses
                    .Where(e => e.ProjectId == projectId
                                && e.CreatedDate >= startDate
                                && e.CreatedDate <= endDate
                                && e.ApprovalStatus == ApprovalStatus.Approved)
                    .Include(e => e.ExpenseType)
                    .ToListAsync();

                // Explicitly specifying the types for GroupBy (group by string for monthly, date for daily/weekly)
                var expenses = rawExpenses
                    .GroupBy(e => chartType == "monthly"
                                  ? e.CreatedDate.ToString("MMMM yyyy")  // Group by month and year for monthly reports
                                  : e.CreatedDate.Date.ToShortDateString()) // Group by the exact date for daily/weekly reports
                    .Select(g => new
                    {
                        DateGroup = g.Key,
                        Total = g.Sum(e => e.ExpenseAmount)
                    })
                    .ToDictionary(g => g.DateGroup, g => g.Total);

                return View("ExpenseChart", expenses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        //[HttpGet("Expense/ShowExpenseChart")]
        //public async Task<IActionResult> ShowExpenseChart(int projectId, string chartType)
        //{
        //    try
        //    {
        //        var project = await dbContext.Projects.FindAsync(projectId);
        //        if (project == null)
        //        {
        //            return NotFound($"Project with ID {projectId} not found.");
        //        }

        //        ViewBag.ChartType = chartType switch
        //        {
        //            "daily" => "Daily",
        //            "weekly" => "Weekly",
        //            "monthly" => "Monthly",
        //            _ => throw new ArgumentException("Invalid chart type.")
        //        };

        //        DateTime startDate;
        //        switch (chartType)
        //        {
        //            case "daily":
        //                startDate = DateTime.Today.AddDays(-1);  // Last day
        //                break;
        //            case "weekly":
        //                startDate = DateTime.Today.AddDays(-7);  // Last 7 days
        //                break;
        //            case "monthly":
        //                startDate = DateTime.Today.AddMonths(-1);  // Last month
        //                break;
        //            default:
        //                throw new ArgumentException("Invalid chart type.");
        //        }

        //        // Fetch expenses and group them by the appropriate time range
        //        var expensesQuery = dbContext.Expenses
        //            .Where(e => e.ProjectId == projectId && e.CreatedDate >= startDate && e.ApprovalStatus == ApprovalStatus.Approved)
        //            .Include(e => e.ExpenseType);

        //        // Grouping and selecting the correct data based on chartType
        //        var expenses = chartType switch
        //        {
        //            "daily" => await expensesQuery
        //                .GroupBy(e => e.CreatedDate.Date)  // Group by exact date
        //                .Select(g => new { Date = g.Key.ToString("dd/MM/yyyy"), Total = g.Sum(e => e.ExpenseAmount) })
        //                .ToDictionaryAsync(g => g.Date, g => g.Total),

        //            "weekly" => await expensesQuery
        //                .GroupBy(e => e.CreatedDate.Date)  // Group by exact date
        //                .Select(g => new { Date = g.Key.ToString("dd/MM/yyyy"), Total = g.Sum(e => e.ExpenseAmount) })
        //                .ToDictionaryAsync(g => g.Date, g => g.Total),

        //            "monthly" => await expensesQuery
        //               .GroupBy(e => new { e.CreatedDate.Year, e.CreatedDate.Month })
        //                .Select(g => new {
        //                    Date = $"{g.Key.Month}/{g.Key.Year}",
        //                    Total = g.Sum(e => e.ExpenseAmount)
        //                })
        //                .ToDictionaryAsync(g => g.Date, g => g.Total),

        //            _ => throw new ArgumentException("Invalid chart type.")
        //        };

        //        return View("ExpenseChart", expenses);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}


        //[HttpGet("Expense/ShowExpenseChart")]
        //public async Task<IActionResult> ShowExpenseChart(int projectId, string chartType)
        //{
        //    try
        //    {
        //        var project = await dbContext.Projects.FindAsync(projectId);
        //        if (project == null)
        //        {
        //            return NotFound($"Project with ID {projectId} not found.");
        //        }

        //        switch (chartType)
        //        {
        //            case "daily":
        //                ViewBag.ChartType = "Daily";
        //                break;
        //            case "weekly":
        //                ViewBag.ChartType = "Weekly";
        //                break;
        //            case "monthly":
        //                ViewBag.ChartType = "Monthly";
        //                break;
        //            default:
        //                throw new ArgumentException("Invalid chart type.");
        //        }

        //        DateTime startDate;
        //        switch (chartType)
        //        {
        //            case "daily":
        //                startDate = DateTime.Today.AddDays(-1);
        //                break;
        //            case "weekly":
        //                startDate = DateTime.Today.AddDays(-7);
        //                break;
        //            case "monthly":
        //                startDate = DateTime.Today.AddMonths(-1);
        //                break;
        //            default:
        //                throw new ArgumentException("Invalid chart type.");
        //        }

        //        var expenses = await dbContext.Expenses
        //            .Where(e => e.ProjectId == projectId && e.CreatedDate >= startDate && e.ApprovalStatus == ApprovalStatus.Approved)
        //            .Include(e => e.ExpenseType)
        //            .GroupBy(e => e.ExpenseType.Name)
        //            .Select(g => new { ExpenseType = g.Key, Total = g.Sum(e => e.ExpenseAmount) })
        //            .ToDictionaryAsync(g => g.ExpenseType, g => g.Total);

        //        return View("ExpenseChart", expenses);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}


    }
    #endregion

}


