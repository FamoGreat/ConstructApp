using ConstructApp.Data;
using ConstructApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;
using ConstructApp.Constants;

namespace ConstructApp.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        public ProjectController(ApplicationDbContext dbContextContext)
        {
            dbContext = dbContextContext;   
        }
        public IActionResult Index()
        {
            List<Project> projectList = dbContext.Projects.ToList();
            return View(projectList);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Project project)
        {
            if (ModelState.IsValid)
            {
                dbContext.Projects.Add(project);
                dbContext.SaveChanges();
                TempData["success"] = "Project created successfully";

                return RedirectToAction("Index", "Project");
            }
            return View();
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Project? project = dbContext.Projects.FirstOrDefault(p => p.Id == id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        [HttpPost]
        public IActionResult Edit(Project updatedProject)
        {
            if (ModelState.IsValid)
            {
                dbContext.Update(updatedProject);
                dbContext.SaveChanges();
                TempData["success"] = "Project updated successfully";

                return RedirectToAction("Index", "Project");
            }
            return View();
        }

        public IActionResult Details(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var project = dbContext.Projects.Include(p => p.ProjectMaterials).Include(pt => pt.ProjectTools).FirstOrDefault(p => p.Id == id);
           
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Project> projects = dbContext.Projects.ToList();

            return Json(new { data = projects });
        }
        [HttpGet]
        public IActionResult GetMaterial(int? id)
        {
            if (id == null || id == 0)
            {
                return BadRequest(new { success = false, message = "ID parameter is missing." });
            }

            var project = dbContext.Projects.Include(p => p.ProjectMaterials).FirstOrDefault(p => p.Id == id);

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
        public IActionResult GetTools(int? id)
        {
            if (id == null || id == 0)
            {
                return BadRequest(new { success = false, message = "ID parameter is missing." });
            }

            var project = dbContext.Projects.Include(p => p.ProjectTools).FirstOrDefault(p => p.Id == id);

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


        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var projectToBeDeleted = dbContext.Projects.FirstOrDefault(p => p.Id == id);
            if (projectToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deliting" });
            }

            dbContext.Remove(projectToBeDeleted);
            dbContext.SaveChanges();
            return Json(new { success = true, message = "Delete Successfully" });
        }


        #endregion
    }
}
