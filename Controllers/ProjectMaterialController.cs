using ConstructApp.Constants;
using ConstructApp.Data;
using ConstructApp.Models;
using ConstructApp.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
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
            var model = new ProjectMaterialVM
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
        public IActionResult GetMaterialsByProject(int projectId)
        {
            var materials = dbContext.ProjectMaterials
                .Where(pm => pm.ProjectId == projectId)
                .Include(pm => pm.Project)
                .ToList();

            return PartialView("_MaterialsTablePartial", materials);
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

                if (!Enum.IsDefined(typeof(UnitOfMeasurement), projectMaterialVM.ProjectMaterial.MaterialUOM))
                {
                    ModelState.AddModelError("MaterialUOM", "Invalid unit of measurement.");
                    return View(projectMaterialVM);
                }

                dbContext.ProjectMaterials.Add(projectMaterialVM.ProjectMaterial);
                dbContext.SaveChanges();
                TempData["success"] = "Project Material created successfully";
                // Update total material expense after adding material
                UpdateTotalMaterialExpense(projectMaterialVM.ProjectMaterial.ProjectId);

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

                existingMaterial.ProjectId = updatedProjectMaterial.ProjectMaterial.ProjectId;
                existingMaterial.MaterialName = updatedProjectMaterial.ProjectMaterial.MaterialName;
                existingMaterial.EstimatedQuantity = updatedProjectMaterial.ProjectMaterial.EstimatedQuantity;
                existingMaterial.EstimatedCost = updatedProjectMaterial.ProjectMaterial.EstimatedCost;
                existingMaterial.MaterialUOM = updatedProjectMaterial.ProjectMaterial.MaterialUOM;
                existingMaterial.MaterialDescription = updatedProjectMaterial.ProjectMaterial.MaterialDescription;

                dbContext.SaveChanges();
                TempData["success"] = "Project Material updated successfully";
                // Update total material expense after editing material
                UpdateTotalMaterialExpense(updatedProjectMaterial.ProjectMaterial.ProjectId);
                return RedirectToAction("Index", "ProjectMaterial");
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred while updating the Project Material: {ex.Message}";
                return View(updatedProjectMaterial);
            }
        }

        private void UpdateTotalMaterialExpense(int projectId)
        {
            var project = dbContext.Projects.FirstOrDefault(p => p.Id == projectId);
            if (project != null)
            {
                decimal totalMaterialCost = dbContext.ProjectMaterials
                    .Where(pm => pm.ProjectId == projectId)
                    .Sum(pm => pm.EstimatedCost * pm.EstimatedQuantity);

                project.TotalMaterialExpense = totalMaterialCost;

                // Save changes to the database
                dbContext.SaveChanges();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int? id, int projectId)
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

                // Update total material expense after deleting material
                UpdateTotalMaterialExpense(projectId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["erro"] = $"An error occurred while deleting the Material: {ex.Message}.";
                return RedirectToAction(nameof(Index));
            }
        }


        [HttpGet]
        public IActionResult SelectMaterials()
        {
            try
            {
                ViewBag.MaterialTypes = dbContext.Materials.Select(m => m.Type).Distinct().ToList();

                var viewModel = new SelectMaterialsViewModel
                {
                    Materials = new List<Material>(),
                    SelectedType = string.Empty,
                    ProjectList = dbContext.Projects
                                            .Select(p => new SelectListItem
                                            {
                                                Value = p.Id.ToString(),
                                                Text = p.ProjectName
                                            })
                                            .ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {

                TempData["error"] = $"An error occurred while loading the material types: {ex.Message}";
                return View();
            }

        }
        [HttpPost]
        public IActionResult SelectMaterials(SelectMaterialsViewModel viewModel)
        {
            try
            {
                ViewBag.MaterialTypes = dbContext.Materials.Select(m => m.Type).Distinct().ToList();

                if (!string.IsNullOrEmpty(viewModel.SelectedType))
                {
                    viewModel.Materials = dbContext.Materials
                        .Where(m => m.Type == viewModel.SelectedType)
                        .ToList();
                }
                else
                {
                    viewModel.Materials = new List<Material>();
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {

                TempData["error"] = $"An error occurred while filtering materials: {ex.Message}";
                return View(viewModel);
            }
         
        }

        [HttpPost]
        public IActionResult AddSelectedMaterials(SelectMaterialsViewModel viewModel)
        {
            try
            {
                var selectedMaterials = dbContext.Materials
               .Where(m => viewModel.SelectedMaterialIds.Contains(m.Id))
               .ToList();

                foreach (var material in selectedMaterials)
                {
                    var projectMaterial = new ProjectMaterial
                    {
                        MaterialCode = material.Code,
                        MaterialName = material.Name,
                        MaterialUOM = material.UnitOfMeasurement,
                        ProjectId = viewModel.ProjectId,
                        // Set other necessary properties
                    };

                    dbContext.ProjectMaterials.Add(projectMaterial);
                }

                dbContext.SaveChanges();
                TempData["success"] = "Materials are added successfully";
                // Update total material expense for the project after adding materials
                UpdateTotalMaterialExpense(viewModel.ProjectId);
                return RedirectToAction("SelectMaterials", "ProjectMaterial");

                //return RedirectToAction("Details", "Project", new { id = viewModel.ProjectId });
            }
            catch (Exception ex)
            {

                TempData["error"] = $"An error occurred while adding selected materials to the project: {ex.Message}";
                return RedirectToAction("Index", "ProjectMaterial");
            }

        }


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ProjectMaterial> projectMaterials = dbContext.ProjectMaterials.ToList();

            var data = projectMaterials.Select(pm => new
            {
                pm.Id,
                pm.ProjectId,
                pm.MaterialCode,
                pm.MaterialName,
                pm.EstimatedQuantity,
                pm.EstimatedCost,
                MaterialUOM = Enum.GetName(typeof(UnitOfMeasurement), pm.MaterialUOM),
                pm.MaterialDescription
            });

            return Ok(new { data });
        }


        #endregion

    }


}

