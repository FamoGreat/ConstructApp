using ConstructApp.Data;
using ConstructApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConstructApp.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        public UserController(ApplicationDbContext _dbContext)
        {
            dbContext = _dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> applicationUsers = dbContext.ApplicationUsers.ToList();

            var userRoles = dbContext.UserRoles.ToList();
            var roles = dbContext.Roles.ToList();

            foreach (var user in applicationUsers)
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
            }

            return Json(new { data = applicationUsers });
        }


        [HttpDelete]
        public IActionResult Delete(string? id)
        {
            var userToBeDeleted = dbContext.ApplicationUsers.FirstOrDefault(p => p.Id == id);
            if (userToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deliting" });
            }

            dbContext.Remove(userToBeDeleted);
            dbContext.SaveChanges();
            return Json(new { success = true, message = "User Deleted Successfully" });
        }
        #endregion
    }
}
