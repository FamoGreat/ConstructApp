using ConstructApp.Data;
using ConstructApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            var project = dbContext.Projects.FirstOrDefault(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        public IActionResult FullDetails(int projectId)
        {
            var project = dbContext.Projects.Include(p => p.ProjectMaterials).FirstOrDefault(p => p.Id == projectId);
            return View(project);
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Project> projects = dbContext.Projects.ToList();

            return Json(new { data = projects });
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
