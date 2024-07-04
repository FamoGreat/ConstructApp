using ConstructApp.Constants;
using ConstructApp.Data;
using ConstructApp.Helpers;
using ConstructApp.Models;
using ConstructApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConstructApp.Controllers
{
    public class MaterialController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        public MaterialController(ApplicationDbContext _dbContext)
        {
            dbContext = _dbContext;
        }
        public IActionResult Index()
        {
            var materialTypes = dbContext.Materials
                                          .Select(m => m.Type)
                                          .Distinct()
                                          .ToList();

            ViewBag.MaterialTypes = materialTypes;
            return View();
        }

        [HttpGet]
        public IActionResult LoadMaterialList(string type)
        {
            IQueryable<Material> materialsQuery = dbContext.Materials;

            if (!string.IsNullOrEmpty(type))
            {
                materialsQuery = materialsQuery.Where(m => m.Type == type);
            }

            var materials = materialsQuery.ToList();
            return PartialView("_MaterialListPartial", materials);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Material material)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isUnique = dbContext.Materials.Any(p => p.Code == material.Code);
                    if (isUnique)
                    {
                        ModelState.AddModelError("MaterialCode", "Material code must be unique.");
                        return View(material);
                    }
                    if (!Enum.IsDefined(typeof(UnitOfMeasurement), material.UnitOfMeasurement))
                    {
                        ModelState.AddModelError("MaterialUOM", "Invalid unit of measurement.");
                        return View(material);
                    }

                    dbContext.Materials.Add(material);
                    dbContext.SaveChanges();

                    ChangeTrackingHelper.LogChanges(null, material, EntityState.Added, "Material created", dbContext, User.Identity.Name);

                    TempData["success"] = "Material created successfully";
                    return RedirectToAction("Index", "Material");
                }
                else
                {
                    return View(material);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred while creating the Project Material: {ex.Message}";
                return View(material);
            }
        }


        public IActionResult Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var material = dbContext.Materials.FirstOrDefault(p => p.Id == id);
            if (material == null)
            {
                return NotFound();
            }
            return View(material);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Material material)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            if (id != material.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingMaterial = dbContext.Materials.FirstOrDefault(m => m.Id == material.Id);
                    if (existingMaterial == null)
                    {
                        return NotFound();
                    }

                    if (existingMaterial.Code != material.Code)
                    {
                        // Check if the new MaterialCode is unique
                        bool isUnique = !dbContext.Materials.Any(m => m.Code == material.Code);

                        if (!isUnique)
                        {
                            ModelState.AddModelError("MaterialCode", "Material code must be unique.");
                            return View(material);
                        }
                    }

                    // Capture the state of the existing entity before any changes
                    var originalMaterial = new Material
                    {
                        Id = existingMaterial.Id,
                        Code = existingMaterial.Code,
                        Name = existingMaterial.Name,
                        Type = existingMaterial.Type,
                        UnitOfMeasurement = existingMaterial.UnitOfMeasurement
                    };

                    existingMaterial.Name = material.Name;
                    existingMaterial.Type = material.Type;
                    existingMaterial.UnitOfMeasurement = material.UnitOfMeasurement;

                    dbContext.SaveChanges();

                    ChangeTrackingHelper.LogChanges(originalMaterial, material, EntityState.Modified, "Material updated", dbContext, User.Identity.Name);

                    TempData["success"] = "Material updated successfully";
                    return RedirectToAction("Index", "Material");
                }
                catch (Exception ex)
                {
                    TempData["error"] = $"An error occurred while updating the Project Material: {ex.Message}";
                    return View(material);
                }
            }
            return View(material);
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var material = await dbContext.Materials.FindAsync(id);
            if (material == null)
            {
                return Json(new { success = false, message = "Material not found." });
            }

            // Capture the state of the entity before deletion
            //var originalMaterial = new Material
            //{
            //    Id = material.Id,
            //    Code = material.Code,
            //    Name = material.Name,
            //    Type = material.Type,
            //    UnitOfMeasurement = material.UnitOfMeasurement
            //};

            dbContext.Materials.Remove(material);
            await dbContext.SaveChangesAsync();

            // Log the deletion
            //ChangeTrackingHelper.LogChanges(originalMaterial, null, EntityState.Deleted, "Material deleted", dbContext, User.Identity.Name);

            return Json(new { success = true, message = "Material deleted successfully." });
        }



    }
}
