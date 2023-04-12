using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Areas.Identity.Data;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Controllers
{
    [Authorize(Roles = "Developer, Administrator")]
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

            List<ApplicationProject> projects = await _context.Projects
                .Where(p => p.Developers.Any(d => d.ApplicationUserId == currentUser.Id))
                .ToListAsync();

            return View(projects);
        }

        public async Task<IActionResult> MyTasks(int projectId)
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(User);

            List<ApplicationTask> tasks = await _context.Tasks
                .Where(t => t.ApplicationProjectId == projectId && t.Developers.Any(d => d.ApplicationUserId == currentUser.Id))
                .ToListAsync();

            return View(tasks);
        }
    }
}
