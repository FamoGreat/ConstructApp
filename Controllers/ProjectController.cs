using ConstructApp.Data;
using ConstructApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ConstructApp.Constants;
using Microsoft.AspNetCore.Identity;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Build.ObjectModelRemoting;
using ConstructApp.Helpers;
using ConstructApp.Models.ViewModels;
using System.Security.Claims;

namespace ConstructApp.Controllers
{

    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthorizationService _authorizationService;

        public ProjectController(ApplicationDbContext _dbContext, UserManager<ApplicationUser> userManager, IAuthorizationService authorizationService)
        {
            dbContext = _dbContext;
            _userManager = userManager;
            _authorizationService = authorizationService;
        }

        public async Task<IActionResult> Index()
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, Constants.Permissions.ProjectPermissions.View);

            if (!authorizationResult.Succeeded)
            {
                return RedirectToAction("Index", "ErrorPermission");
            }

            string currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            List<Project> projectList = await dbContext.Projects.ToListAsync();

            return View(projectList);
        }


        public async Task<IActionResult> Create()
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, Constants.Permissions.ProjectPermissions.Create);

            if (!authorizationResult.Succeeded)
            {
                return RedirectToAction("Index", "ErrorPermission");
            }

            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                var fullName = $"{user.FirstName} {user.LastName}";

                var project = new Project
                {
                    CreatedBy = fullName
                };

                return View(project);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(Project project)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    project.TotalMaterialExpense = 0;
                    project.TotalToolExpense = 0;

                    dbContext.Projects.Add(project);
                    await dbContext.SaveChangesAsync();

                    TempData["success"] = "Project created successfully";
                    ChangeTrackingHelper.LogChanges<Project>(null, project, EntityState.Added, "Project created", dbContext, User.Identity.Name);

                    return RedirectToAction(nameof(Index));
                }

                return View(project);
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred while creating the project: {ex.Message}";

                return RedirectToAction("Error", "Home");
            }
        }


        public async Task<IActionResult> Edit(int? projectId)
        {

            try
            {
                if (projectId == null || projectId == 0)
                {
                    return NotFound();
                }

                var authorizationResult = await _authorizationService.AuthorizeAsync(User, Constants.Permissions.ProjectPermissions.Edit);
                if (!authorizationResult.Succeeded)
                {
                    return RedirectToAction("Index", "ErrorPermission");
                }

                var project = await dbContext.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
                if (project == null)
                {
                    return NotFound();
                }

                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var fullName = $"{user.FirstName} {user.LastName}";
                    project.CreatedBy = fullName;
                }

                return View(project);
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred while retrieving the project for editing: {ex.Message}";
                return View();
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Project updatedProject)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var originalProject = await dbContext.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == updatedProject.Id);

                    ChangeTrackingHelper.LogChanges<Project>(originalProject, updatedProject, EntityState.Modified, "Project Updated", dbContext, User.Identity.Name);

                    dbContext.Update(updatedProject);
                    await dbContext.SaveChangesAsync();
                    TempData["success"] = "Project updated successfully";

                    return RedirectToAction(nameof(Index));
                }

                return View(updatedProject);
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred while updating the project: {ex.Message}";
                return View(updatedProject);
            }
        }

        public async Task<IActionResult> Details(int? projectId)
        {
            if (projectId == null || projectId == 0)
            {
                return NotFound();
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, Constants.Permissions.ProjectPermissions.View);
            if (!authorizationResult.Succeeded)
            {
                return RedirectToAction("ErrorPermission", "Permission");
            }

            var project = await dbContext.Projects
                                         .Include(p => p.ProjectMaterials)
                                         .Include(pt => pt.ProjectTools)
                                         .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
            {
                return NotFound();
            }

            // Calculate total material and tool expenses
            UpdateTotalExpenses(project.Id);

            return View(project);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, Constants.Permissions.ProjectPermissions.Delete);
            if (!authorizationResult.Succeeded)
            {
                return RedirectToAction("Index", "ErrorPermission");
            }

            var project = await dbContext.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            try
            {
                dbContext.Projects.Remove(project);
                await dbContext.SaveChangesAsync();

                TempData["success"] = "Project deleted successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred while deleting the project: {ex.Message}.";
                return RedirectToAction(nameof(Index));
            }
        }


        private void UpdateTotalExpenses(int projectId)
        {
            var project = dbContext.Projects.FirstOrDefault(p => p.Id == projectId);
            if (project != null)
            {
                // Calculate total material cost
                decimal totalMaterialCost = dbContext.ProjectMaterials
                    .Where(pm => pm.ProjectId == projectId)
                    .Sum(pm => pm.EstimatedCost * pm.EstimatedQuantity);

                // Calculate total tool cost
                decimal totalToolCost = dbContext.ProjectTools
                    .Where(pt => pt.ProjectId == projectId)
                    .Sum(pt => pt.ToolCost * pt.ToolsQuantity);

                // Update project's total expenses
                project.TotalMaterialExpense = totalMaterialCost;
                project.TotalToolExpense = totalToolCost;

                // Save changes to the database
                dbContext.SaveChanges();
            }
        }


        #region API CALLS

        [HttpGet]
        public async Task<IActionResult> GetMaterial(int? id)
        {
            if (id == null || id == 0)
            {
                return BadRequest(new { success = false, message = "ID parameter is missing." });
            }

            var project = await dbContext.Projects.Include(p => p.ProjectMaterials).FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound(new { success = false, message = $"Project Material with ID {id} not found." });
            }

            foreach (var material in project.ProjectMaterials)
            {
                material.MaterialUOMString = Enum.GetName(typeof(UnitOfMeasurement), material.MaterialUOM);
            }

            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true
            };

            return Json(new { data = project.ProjectMaterials }, options);
        }

        [HttpGet]
        public async Task<IActionResult> GetTools(int? id)
        {
            if (id == null || id == 0)
            {
                return BadRequest(new { success = false, message = "ID parameter is missing." });
            }

            var project = await dbContext.Projects.Include(p => p.ProjectTools).FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound(new { success = false, message = $"Project with ID {id} not found." });
            }

            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true
            };

            return Json(new { data = project.ProjectTools }, options);
        } 

        #endregion
    }
}
