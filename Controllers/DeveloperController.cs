using Microsoft.AspNetCore.Mvc;

namespace TaskManagementSystem.Controllers
{
    public class DeveloperController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
