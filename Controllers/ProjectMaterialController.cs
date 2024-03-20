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
        public IActionResult Index(int id)
        {
            List<ProjectMaterial> ProjectMaterialList = dbContext.ProjectMaterials.Where(p => p.ProjectId == id).ToList();
            return View(ProjectMaterialList);
        }
        public IActionResult Create(int id)
        {

            var projectMaterail = new ProjectMaterial {ProjectId = id };
            return View(projectMaterail);
        }
        [HttpPost]
        public IActionResult Create(ProjectMaterial ProjectMaterial)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isUnique = dbContext.ProjectMaterials.Any(p => p.MaterialCode == ProjectMaterial.MaterialCode);
                    if (isUnique)
                    {
                        // Material code is not unique, add a model error and return the view
                        ModelState.AddModelError("MaterialCode", "Material code must be unique.");
                        return View(ProjectMaterial);
                    }
                    ProjectMaterial.Id = 0;
                    dbContext.ProjectMaterials.Add(ProjectMaterial);
                    dbContext.SaveChanges();
                    TempData["success"] = "Project Material created successfully";

                    return RedirectToAction("Index", "ProjectMaterial");
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred while creating the Project Material: {ex.Message}";
            }

            return View();
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
            if (ModelState.IsValid)
            {
                dbContext.Update(updatedProjectMaterial);
                dbContext.SaveChanges();
                TempData["success"] = "Project Material updated successfully";

                return RedirectToAction("Index", "ProjectMaterial");
            }
            return View();
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
        public IActionResult GetAll(int id)
        {
            List<ProjectMaterial> ProjectMaterials = dbContext.ProjectMaterials.Where(p => p.ProjectId == id).ToList();
            return Ok(new { data = ProjectMaterials });
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
                return Ok(new { success = true, message = "Delete Successful" });
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

