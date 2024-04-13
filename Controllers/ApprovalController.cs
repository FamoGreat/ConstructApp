using Microsoft.AspNetCore.Mvc;

namespace ConstructApp.Controllers
{
    public class ApprovalController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
