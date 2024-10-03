using ConstructApp.Data;
using ConstructApp.Helpers;
using ConstructApp.Models;
using ConstructApp.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ConstructApp.Controllers
{

    public class ProjectToolController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        public ProjectToolController(ApplicationDbContext dbContextContext, UserManager<ApplicationUser> userManager)
        {
            dbContext = dbContextContext;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var model = new ProjectToolVM
            {
                ProjectList = dbContext.Projects
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.ProjectName
                }).ToList()
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult GetToolsByProject(int projectId)
        {
            var tools = dbContext.ProjectTools
                .Where(pt => pt.ProjectId == projectId)
                .Include(pt => pt.Project)
                .ToList();

            return PartialView("_ToolsTablePartial", tools);
        }

        public async Task<IActionResult> Create()
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            ProjectToolVM projectToollVM = new()
            {
                ProjectList = dbContext.Projects.Select(u => new SelectListItem
                {
                    Text = u.ProjectName,
                    Value = u.Id.ToString()
                })
            };

            return View(projectToollVM);
        }
    

    [HttpPost]
        public IActionResult Create(ProjectTool projectTool)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dbContext.ProjectTools.Add(projectTool);
                    dbContext.SaveChanges();
                    TempData["success"] = "Project Tool created successfully";

                    ChangeTrackingHelper.LogChanges<ProjectTool>(null, projectTool, EntityState.Added, "Project Tool created", dbContext, User.Identity.Name);

                    UpdateTotalToolExpense(projectTool.ProjectId);
                    return RedirectToAction("Index", "ProjectTool");
                }
                return View(projectTool);
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred while creating the Project Tool: {ex.Message}";
                return View(projectTool);
            }
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            ProjectTool? projectTool = dbContext.ProjectTools.FirstOrDefault(p => p.Id == id);
            if (projectTool == null)
            {
                return NotFound();
            }

            var viewModel = new ProjectToolVM
            {
                ProjectTool = projectTool,
                ProjectList = dbContext.Projects.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.ProjectName
                }).ToList()
            };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProjectToolVM viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.ProjectList = dbContext.Projects.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.ProjectName
                }).ToList();
                return View(viewModel);
            }

            try
            {
                var existingTool = dbContext.ProjectTools.FirstOrDefault(p => p.Id == viewModel.ProjectTool.Id);
                if (existingTool == null)
                {
                    return NotFound();
                }

                ChangeTrackingHelper.LogChanges<ProjectTool>(existingTool, viewModel.ProjectTool, EntityState.Modified, viewModel.UpdateReason, dbContext, User.Identity.Name);

                existingTool.ToolName = viewModel.ProjectTool.ToolName;
                existingTool.ToolDescription = viewModel.ProjectTool.ToolDescription;
                existingTool.ToolsQuantity = viewModel.ProjectTool.ToolsQuantity;
                existingTool.ToolCost = viewModel.ProjectTool.ToolCost;
                existingTool.ProjectId = viewModel.ProjectTool.ProjectId;

                dbContext.SaveChanges();
                TempData["success"] = "Project Tool updated successfully";

                UpdateTotalToolExpense(viewModel.ProjectTool.ProjectId);

                return RedirectToAction("Index", "ProjectTool");
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred while updating the Project Tool: {ex.Message}";

                viewModel.ProjectList = dbContext.Projects.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.ProjectName
                }).ToList();
                return View(viewModel);
            }
        }

        private void UpdateTotalToolExpense(int projectId)
        {
            var project = dbContext.Projects.FirstOrDefault(p => p.Id == projectId);
            if (project != null)
            {
                decimal totalToolCost = dbContext.ProjectTools
                    .Where(pt => pt.ProjectId == projectId)
                    .Sum(pt => pt.ToolCost * pt.ToolsQuantity);

                project.TotalToolExpense = totalToolCost;

                dbContext.SaveChanges();
            }
        }


        #region API CALLS
        [HttpDelete("{id}")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest(new { success = false, message = "ID parameter is missing." });
            }

            var projectToolToBeDeleted = dbContext.ProjectTools.FirstOrDefault(p => p.Id == id);
            if (projectToolToBeDeleted == null)
            {
                return NotFound(new { success = false, message = $"Project Tool with ID {id} not found." });
            }

            // Capture the state of the entity before deletion
            //var originalProjectTool = new ProjectTool
            //{
            //    Id = projectToolToBeDeleted.Id,
            //    ToolName = projectToolToBeDeleted.ToolName,
            //    ToolDescription = projectToolToBeDeleted.ToolDescription,
            //    ToolsQuantity = projectToolToBeDeleted.ToolsQuantity,
            //    ToolCost = projectToolToBeDeleted.ToolCost,
            //    ProjectId = projectToolToBeDeleted.ProjectId
            //};

            try
            {
                dbContext.ProjectTools.Remove(projectToolToBeDeleted);
                dbContext.SaveChanges();

                UpdateTotalToolExpense(projectToolToBeDeleted.ProjectId);

                // Log the deletion
                //ChangeTrackingHelper.LogChanges(originalProjectTool, null, EntityState.Deleted, "Project Tool deleted", dbContext, User.Identity.Name);

                return Ok(new { success = true, message = "Tool Delete Successful" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred while deleting the project Tool: {ex.Message}" });
            }
        }



        #endregion

    }


}

