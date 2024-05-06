using ConstructApp.Constants;
using ConstructApp.Data;
using ConstructApp.Models;
using ConstructApp.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ConstructApp.Controllers
{

    public class ProjectMaterialController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        public ProjectMaterialController(ApplicationDbContext dbContextContext, UserManager<ApplicationUser> userManager)
        {
            dbContext = dbContextContext;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            List<ProjectMaterial> ProjectMaterialList = dbContext.ProjectMaterials
                .Include(p => p.Project)
                .ToList();
            return View(ProjectMaterialList);
        }
        public async Task<IActionResult> Create()
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            ProjectMaterialVM projectMaterialVM = new()
            {
                ProjectList = dbContext.Projects.Select(u => new SelectListItem
                {
                    Text = u.ProjectName,
                    Value = u.Id.ToString()
                })
            };

            return View(projectMaterialVM);
        }
      
        [HttpPost]
        public IActionResult Create(ProjectMaterialVM projectMaterialVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(projectMaterialVM);
                }

                // Check if the MaterialCode is unique
                bool isUnique = dbContext.ProjectMaterials.Any(p => p.MaterialCode == projectMaterialVM.ProjectMaterial.MaterialCode);
                if (isUnique)
                {
                    ModelState.AddModelError("MaterialCode", "Material code must be unique.");
                    return View(projectMaterialVM);
                }

                // Validate MaterialUOM enum value
                if (!Enum.IsDefined(typeof(UnitOfMeasurement), projectMaterialVM.ProjectMaterial.MaterialUOM))
                {
                    ModelState.AddModelError("MaterialUOM", "Invalid unit of measurement.");
                    return View(projectMaterialVM);
                }

                dbContext.ProjectMaterials.Add(projectMaterialVM.ProjectMaterial);
                dbContext.SaveChanges();
                TempData["success"] = "Project Material created successfully";

                return RedirectToAction("Index", "ProjectMaterial");
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred while creating the Project Material: {ex.Message}";
                return View(projectMaterialVM);
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

                ProjectMaterial? projectMaterial = dbContext.ProjectMaterials.FirstOrDefault(p => p.Id == id);
                if (projectMaterial == null)
                {
                    return NotFound();
                }
                var projectList = dbContext.Projects.Select(u => new SelectListItem
                {
                    Text = u.ProjectName,
                    Value = u.Id.ToString(),
                    Selected = u.Id == projectMaterial.ProjectId
                }).ToList();
                var projectMaterialVM = new ProjectMaterialVM
                {
                    ProjectMaterial = projectMaterial,
                    ProjectList = projectList
                };
                return View(projectMaterialVM);
            }
            catch (Exception ex)
            {
                TempData["error"] = "An error occurred: " + ex.Message;
                return RedirectToAction("Index");
            }
           
        }

        [HttpPost]
        public IActionResult Edit(ProjectMaterialVM updatedProjectMaterial)
        {
            if (!ModelState.IsValid)
            {
                return View(updatedProjectMaterial);
            }

            try
            {
                var existingMaterial = dbContext.ProjectMaterials.FirstOrDefault(p => p.Id == updatedProjectMaterial.ProjectMaterial.Id);
                if (existingMaterial == null)
                {
                    return NotFound();
                }

                if (existingMaterial.MaterialCode != updatedProjectMaterial.ProjectMaterial.MaterialCode)
                {
                    // Check if the new MaterialCode is unique
                    bool isUnique = !dbContext.ProjectMaterials.Any(p => p.MaterialCode == updatedProjectMaterial.ProjectMaterial.MaterialCode);

                    if (!isUnique)
                    {
                        ModelState.AddModelError("MaterialCode", "Material code must be unique.");
                        return View(updatedProjectMaterial);
                    }
                }

                // Update other properties
                existingMaterial.MaterialName = updatedProjectMaterial.ProjectMaterial.MaterialName;
                existingMaterial.EstimatedQuantity = updatedProjectMaterial.ProjectMaterial.EstimatedQuantity;
                existingMaterial.EstimatedCost = updatedProjectMaterial.ProjectMaterial.EstimatedCost;
                existingMaterial.MaterialUOM = updatedProjectMaterial.ProjectMaterial.MaterialUOM;
                // Update ProjectName if necessary
                //if (existingMaterial.ProjectId != updatedProjectMaterial.ProjectMaterial.ProjectId)
                //{
                //    var project = dbContext.Projects.FirstOrDefault(p => p.Id == updatedProjectMaterial.ProjectMaterial.ProjectId);
                //    if (project != null)
                //    {
                //        if (existingMaterial.Project == null)
                //        {
                //            existingMaterial.Project = new Project(); 
                //        }

                //        existingMaterial.Project.ProjectName = project.ProjectName;

                //        if (User.Identity.IsAuthenticated)
                //        {
                //            existingMaterial.Project.CreatedBy = User.Identity.Name;
                //        }
                //    }
                //}

                dbContext.SaveChanges();
                TempData["success"] = "Project Material updated successfully";

                return RedirectToAction("Index", "ProjectMaterial");
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred while updating the Project Material: {ex.Message}";
                return View(updatedProjectMaterial);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest(new { success = false, message = "ID parameter is missing." });
            }

            var ProjectMaterialToBeDeleted = dbContext.ProjectMaterials.FirstOrDefault(p => p.Id == id);
            if (ProjectMaterialToBeDeleted == null)
            {
                return NotFound(new { success = false, message = $"Project Material with ID {id} not found." });
            }

            try
            {
                dbContext.Remove(ProjectMaterialToBeDeleted);
                dbContext.SaveChanges();
                TempData["success"] = "Material Delete Successful";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["erro"] = $"An error occurred while deleting the Material: {ex.Message}.";
                return RedirectToAction(nameof(Index));
            }
        }


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ProjectMaterial> projectMaterials = dbContext.ProjectMaterials.ToList();

            // Convert enum values to string representations
            var data = projectMaterials.Select(pm => new
            {
                pm.Id,
                pm.MaterialCode,
                pm.MaterialName,
                pm.EstimatedQuantity,
                pm.EstimatedCost,
                MaterialUOM = Enum.GetName(typeof(UnitOfMeasurement), pm.MaterialUOM),
                pm.ProjectId
            });

            return Ok(new { data });
        }


        #endregion

    }


}

