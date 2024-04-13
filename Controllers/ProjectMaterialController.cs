using ConstructApp.Constants;
using ConstructApp.Data;
using ConstructApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConstructApp.Controllers
{
    public class ProjectMaterialController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        public ProjectMaterialController(ApplicationDbContext dbContextContext)
        {
            dbContext = dbContextContext;
        }
        public IActionResult Index()
        {
            List<ProjectMaterial> ProjectMaterialList = dbContext.ProjectMaterials.ToList();
            return View(ProjectMaterialList);
        }
        public IActionResult Create(int id)
        {

            var projectMaterail = new ProjectMaterial {ProjectId = id };
            return View(projectMaterail);
        }
        [HttpPost]
        //public IActionResult Create(ProjectMaterial ProjectMaterial)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            bool isUnique = dbContext.ProjectMaterials.Any(p => p.MaterialCode == ProjectMaterial.MaterialCode);
        //            if (isUnique)
        //            {
        //                // Material code is not unique, add a model error and return the view
        //                ModelState.AddModelError("MaterialCode", "Material code must be unique.");
        //                return View(ProjectMaterial);
        //            }
        //            ProjectMaterial.Id = 0;
        //            dbContext.ProjectMaterials.Add(ProjectMaterial);
        //            dbContext.SaveChanges();
        //            TempData["success"] = "Project Material created successfully";

        //            return RedirectToAction("Index", "ProjectMaterial");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["error"] = $"An error occurred while creating the Project Material: {ex.Message}";
        //    }

        //    return View();
        //}
        [HttpPost]
        public IActionResult Create(ProjectMaterial ProjectMaterial)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(ProjectMaterial);
                }

                // Check if the MaterialCode is unique
                bool isUnique = dbContext.ProjectMaterials.Any(p => p.MaterialCode == ProjectMaterial.MaterialCode);
                if (isUnique)
                {
                    ModelState.AddModelError("MaterialCode", "Material code must be unique.");
                    return View(ProjectMaterial);
                }

                // Validate MaterialUOM enum value
                if (!Enum.IsDefined(typeof(UnitOfMeasurement), ProjectMaterial.MaterialUOM))
                {
                    ModelState.AddModelError("MaterialUOM", "Invalid unit of measurement.");
                    return View(ProjectMaterial);
                }

                ProjectMaterial.Id = 0;
                dbContext.ProjectMaterials.Add(ProjectMaterial);
                dbContext.SaveChanges();
                TempData["success"] = "Project Material created successfully";

                return RedirectToAction("Index", "ProjectMaterial");
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred while creating the Project Material: {ex.Message}";
                return View(ProjectMaterial);
            }
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            ProjectMaterial? ProjectMaterial = dbContext.ProjectMaterials.FirstOrDefault(p => p.Id == id);
            if (ProjectMaterial == null)
            {
                return NotFound();
            }

            return View(ProjectMaterial);
        }

        [HttpPost]
        public IActionResult Edit(ProjectMaterial updatedProjectMaterial)
        {
            if (!ModelState.IsValid)
            {
                return View(updatedProjectMaterial);
            }

            try
            {
                var existingMaterial = dbContext.ProjectMaterials.FirstOrDefault(p => p.Id == updatedProjectMaterial.Id);
                if (existingMaterial == null)
                {
                    return NotFound();
                }

                if (existingMaterial.MaterialCode != updatedProjectMaterial.MaterialCode)
                {
                    // Check if the new MaterialCode is unique
                    bool isUnique = !dbContext.ProjectMaterials.Any(p => p.MaterialCode == updatedProjectMaterial.MaterialCode);

                    // If the new MaterialCode is not unique, add a ModelState error and return the view
                    if (!isUnique)
                    {
                        ModelState.AddModelError("MaterialCode", "Material code must be unique.");
                        return View(updatedProjectMaterial);
                    }
                }
                
                // Update other properties
                existingMaterial.MaterialName = updatedProjectMaterial.MaterialName;
                existingMaterial.EstimatedQuantity = updatedProjectMaterial.EstimatedQuantity;
                existingMaterial.EstimatedCost = updatedProjectMaterial.EstimatedCost;
                existingMaterial.MaterialUOM = updatedProjectMaterial.MaterialUOM;

                dbContext.SaveChanges();
                TempData["success"] = "Project Material updated successfully";

                return RedirectToAction("Details", "Project", new { id = existingMaterial.ProjectId });
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred while updating the Project Material: {ex.Message}";
                return View(updatedProjectMaterial);
            }
        }


        public IActionResult Details(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var ProjectMaterial = dbContext.ProjectMaterials.FirstOrDefault(p => p.Id == id);

            if (ProjectMaterial == null)
            {
                return NotFound();
            }

            return View(ProjectMaterial);
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ProjectMaterial> projectMaterials = dbContext.ProjectMaterials.ToList();

            // Convert enum values to string representations
            var data = projectMaterials.Select(pm => new {
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

        [HttpDelete]
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
                return Ok(new { success = true, message = "Material Delete Successful" });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { success = false, message =$"An error occurred while deleting the project material: {ex.Message}" });
            }
        }

        #endregion

    }

   
}

