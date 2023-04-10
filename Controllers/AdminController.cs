using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManagementSystem.Areas.Identity.Data;
using TaskManagementSystem.Models.ViewModels;

namespace TaskManagementSystem.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationContext _context;

        private readonly IEnumerable<SelectListItem> _assignableRoles = new List<SelectListItem>()
        {
            new SelectListItem("Developer", "Developer"),
            new SelectListItem("Project Manager", "ProjectManager")
        };

        public AdminController(UserManager<ApplicationUser> userManager, ApplicationContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<ApplicationUser> users = new();

            foreach (ApplicationUser user in _context.Users.AsEnumerable())
                if ((await _userManager.GetRolesAsync(user)).Count == 0)
                    users.Add(user);

            return View(new AssignRoleVM()
            {
                Users = users,
                Roles = _assignableRoles
            });
        }

        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(string userId, [Bind("Role")] AssignRoleVM vm)
        {
            ApplicationUser? user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound();

            if (!(await _userManager.AddToRoleAsync(user, vm.Role)).Succeeded)
                ModelState.AddModelError("", "Could not add user to role.");

            return RedirectToAction("Index");
        }
    }
}