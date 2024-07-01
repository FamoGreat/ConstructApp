using Microsoft.AspNetCore.Mvc;

namespace ConstructApp.Controllers
{
    public class ToolController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
