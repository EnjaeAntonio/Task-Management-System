using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManagementSystem.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Build.Evaluation;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Controllers
{
    [Authorize(Roles = "Developer")]
    public class DeveloperController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationContext _context;

        public DeveloperController(UserManager<ApplicationUser> userManager, ApplicationContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        public async Task<IActionResult> MyProjects()
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(User);

            List<Projects> projects = _context.UserProjects
                .Include(up => up.Project)
                .Where(up => up.ApplicationUserId == currentUser.Id)
                .Select(up => up.Project)
                .ToList();

            return View(projects);
        }
    }
}
