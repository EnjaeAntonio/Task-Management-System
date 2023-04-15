using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Areas.Identity.Data;
using TaskManagementSystem.Models;
using TaskManagementSystem.Models.ViewModels;

namespace TaskManagementSystem.Controllers
{
    [Authorize(Roles ="Project Manager, Administrator")]
    public class ProjectsController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectsController(ApplicationContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
            _userManager = userManager;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            string currentUserId = _userManager.GetUserId(User);

            var ProjectTasks = await _context.Projects
              .Where(t => t.Developers.Any(td => td.User.UserName == User.Identity.Name))
              .Include(t => t.Tasks)
              .ToListAsync();

            return View(ProjectTasks);
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var projects = await _context.Projects
                .FirstOrDefaultAsync(m => m.Id == id);

            if (projects == null)
            {
                return NotFound();
            }

            return View(projects);
        }

        // GET: Projects/Create
        // GET: Projects/Create
        public async Task<IActionResult> Create()
        {
            ApplicationUser ProjectManager = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

            var developers = await _userManager.GetUsersInRoleAsync("Developer");
            ViewBag.ProjectManagerId = ProjectManager.Id;

            CreateProjectViewModel vm = new CreateProjectViewModel
            {
                Developers = developers,
                ApplicationUserId = ProjectManager.Id,
                ProjectManager = ProjectManager
            };
            return View(vm);
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProjectViewModel CreateProject)
        {
            HashSet <ProjectDeveloper> projectDevelopers = new HashSet <ProjectDeveloper> ();

            ApplicationProject projects = new ApplicationProject
            {
                Title = CreateProject.Title,
                ApplicationUserId = CreateProject.ApplicationUserId,
                ProjectManager = await _context.Users.FindAsync(CreateProject.ApplicationUserId),
                Developers = projectDevelopers,
            };

            foreach (string userId in CreateProject.SelectedDevelopersIdList)
            {
                var projectDeveloper = new ProjectDeveloper
                {
                    ApplicationUserId =  userId,
                    ApplicationProjectId = projects.Id,
                };

                _context.ProjectDevelopers.Add(projectDeveloper);

                projects.Developers.Add(projectDeveloper);
            };


            _context.Add(projects);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var projects = await _context.Projects.FindAsync(id);
            if (projects == null)
            {
                return NotFound();
            }
            return View(projects);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title")] ApplicationProject projects)
        {
            if (id != projects.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(projects);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectsExists(projects.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(projects);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var projects = await _context.Projects
                .FirstOrDefaultAsync(m => m.Id == id);

            ViewBag.Count = projects.Developers.Count();

            if (projects == null)
            {
                return NotFound();
            }

            return View(projects);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Projects == null)
            {
                return Problem("Entity set 'ApplicationContext.Projects'  is null.");
            }
            var projects = await _context.Projects.FindAsync(id);
            if (projects != null)
            {
                _context.Projects.Remove(projects);
            }

            ViewBag.Count = projects.Developers.Count();

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectsExists(int id)
        {
          return (_context.Projects?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
