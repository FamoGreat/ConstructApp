using Microsoft.AspNetCore.Mvc;

namespace ConstructApp.Controllers
{
    public class ErrorPermissionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
