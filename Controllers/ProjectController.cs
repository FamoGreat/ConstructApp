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

namespace ConstructApp.Controllers
{

    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectController(ApplicationDbContext _dbContext, UserManager<ApplicationUser> userManager)
        {
            dbContext = _dbContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            List<Project> projectList = await dbContext.Projects.ToListAsync();
            return View(projectList);
        }

        public async Task<IActionResult> Create()
        {
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
            if (ModelState.IsValid)
            {
                dbContext.Projects.Add(project);
                await dbContext.SaveChangesAsync();
                TempData["success"] = "Project created successfully";

                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        public async Task<IActionResult> Edit(int? projectId)
        {
            try
            {
                if (projectId == null || projectId == 0)
                {
                    return NotFound();
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
        public async Task<IActionResult> Edit(Project updatedProject)
        {
            if (ModelState.IsValid)
            {
                dbContext.Update(updatedProject);
                await dbContext.SaveChangesAsync();
                TempData["success"] = "Project updated successfully";

                return RedirectToAction(nameof(Index));
            }
            return View(updatedProject);
        }

        public async Task<IActionResult> Details(int? projectId)
        {
            if (projectId == null || projectId == 0)
            {
                return NotFound();
            }

            var project = await dbContext.Projects.Include(p => p.ProjectMaterials)
                                                   .Include(pt => pt.ProjectTools)
                                                   .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
            {
                return NotFound();
            }

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
                TempData["erro"] = $"An error occurred while deleting the project: {ex.Message}.";
                return RedirectToAction(nameof(Index));
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
