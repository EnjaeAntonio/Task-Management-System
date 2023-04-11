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

            List<Projects> projects = await _context.UserProjects
                .Include(up => up.Project)
                .Where(up => up.ApplicationUserId == currentUser.Id)
                .Select(up => up.Project)
                .ToListAsync();

            return View(projects);
        }

        public async Task<IActionResult> MyTasks(int projectId)
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(User);

            List<Tasks> tasks = _context.Tasks
                .Where(t => t.ProjectId == projectId && t.Developers.Any(d => d.Id == currentUser.Id))
                .ToList();

            return View(tasks);
        }
    }
}
