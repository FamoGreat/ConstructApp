using ConstructApp.Data;
using ConstructApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConstructApp.Controllers
{

    public class ProjectToolController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        public ProjectToolController(ApplicationDbContext dbContextContext)
        {
            dbContext = dbContextContext;
        }

        public IActionResult Create(int id)
        {

            var projectTool = new ProjectTool { ProjectId = id };
            return View(projectTool);
        }

        [HttpPost]
        public IActionResult Create(ProjectTool projectTool)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(projectTool);
                }

                projectTool.Id = 0;
                // Check if referenced Project exists
                var existingProject = dbContext.Projects.FirstOrDefault(p => p.Id == projectTool.ProjectId);

                if (existingProject == null)
                {
                    TempData["error"] = "The selected Project does not exist.";
                    return View(projectTool);
                }
                dbContext.ProjectTools.Add(projectTool);
                dbContext.SaveChanges();
                TempData["success"] = "Project Tool created successfully";

                return RedirectToAction("Index", "Project");
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred while creating the Project Material: {ex.Message}";
                return View(projectTool);
            }
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            ProjectTool? ProjectTool = dbContext.ProjectTools.FirstOrDefault(p => p.Id == id);
            if (ProjectTool == null)
            {
                return NotFound();
            }

            return View(ProjectTool);
        }

        [HttpPost]
        public IActionResult Edit(ProjectTool updatedProjectTool)
        {
            if (!ModelState.IsValid)
            {
                return View(updatedProjectTool);
            }

            try
            {
                // Avoid potential duplicate tracking
                dbContext.Entry(updatedProjectTool).State = EntityState.Detached;

                var existingTool = dbContext.ProjectTools.FirstOrDefault(p => p.Id == updatedProjectTool.Id);
                if (existingTool == null)
                {
                    return NotFound();
                }

                // Update properties
                existingTool.ToolName = updatedProjectTool.ToolName; // Update other relevant properties
                dbContext.Update(existingTool);
                dbContext.SaveChanges();
                TempData["success"] = "Project Tool updated successfully";

                return RedirectToAction("Details", "Project", new { id = existingTool.ProjectId });
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred while updating the Project Tool: {ex.Message}";
                return View(updatedProjectTool);
            }
        }


        #region API CALLS

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest(new { success = false, message = "ID parameter is missing." });
            }

            var ProjectToolToBeDeleted = dbContext.ProjectTools.FirstOrDefault(p => p.Id == id);
            if (ProjectToolToBeDeleted == null)
            {
                return NotFound(new { success = false, message = $"Project Tool with ID {id} not found." });
            }

            try
            {
                dbContext.Remove(ProjectToolToBeDeleted);
                dbContext.SaveChanges();
                return Ok(new { success = true, message = "Tool Delete Successful" });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { success = false, message = $"An error occurred while deleting the project Tool: {ex.Message}" });
            }
        }

        #endregion

    }


}

