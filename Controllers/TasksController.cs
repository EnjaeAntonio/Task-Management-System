using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Areas.Identity.Data;
using TaskManagementSystem.Models;
using TaskManagementSystem.Models.ViewModels;

namespace TaskManagementSystem.Controllers
{
    public class TasksController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TasksController(ApplicationContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Project Manager")]
        public async Task<IActionResult> Assign(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ApplicationTask? task = await _context.Tasks
                .Include(t => t.Developers)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            IList<ApplicationUser> developers = await _context.ProjectDevelopers
                .Where(pd => pd.ApplicationProjectId == task.ApplicationProjectId)
                .Select(pd => pd.User)
                .ToListAsync();

            AssignTaskViewModel viewModel = new()
            {
                Task = task,
                Developers = developers,
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Project Manager")]
        public async Task<IActionResult> Assign(AssignTaskViewModel vm)
        {
            ApplicationTask? task = await _context.Tasks.FindAsync(vm.TaskId);

            if (task == null)
            {
                return NotFound();
            }

            //check if the user is allocated to the project
            if (!_context.ProjectDevelopers.Any(pd => pd.ApplicationProjectId == task.ApplicationProjectId 
                    && pd.ApplicationUserId == vm.SelectedDeveloperId))
            {
                return BadRequest();
            }

            TaskDeveloper taskDeveloper = new()
            {
                ApplicationTaskId = task.Id,
                ApplicationUserId = vm.SelectedDeveloperId
            };

            _context.TaskDevelopers.Add(taskDeveloper);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Projects");
        }

        // GET: Tasks/Details/5
        [Authorize(Roles = "Project Manager, Developer")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ApplicationTask? task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // GET: Tasks/Create
        [Authorize(Roles = "Project Manager")]
        public async Task<IActionResult> Create(int? projectId)
        {
            if (projectId == null)
            {
                return BadRequest();
            }

            ApplicationProject? project = await _context.Projects.FindAsync(projectId);

            if (project == null)
            {
                return NotFound();
            }

            ViewBag.ProjectId = project.Id;
            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Project Manager")]
        public async Task<IActionResult> Create([Bind("Title,RequiredHours,Completed,Priority,ApplicationProjectId")] ApplicationTask task)
        {
            ApplicationProject? project = await _context.Projects.FindAsync(task.ApplicationProjectId);

            if (project == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Projects");
            }

            ViewBag.ProjectId = task.ApplicationProjectId;
            return View(task);
        }


        // GET: Tasks/Edit/5
        [Authorize(Roles = "Project Manager, Developer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ApplicationTask? task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Tasks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Project Manager, Developer")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,RequiredHours,Completed,Priority,ApplicationProjectId")] ApplicationTask task)
        {
            if (id != task.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Tasks.Update(task);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TasksExists(task.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Projects");
            }

            return View(task);
        }

        // GET: Tasks/Delete/5
        [Authorize(Roles = "Project Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            ApplicationTask? task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Project Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ApplicationTask? task = await _context.Tasks.FindAsync(id);

            if (task != null)
            {
                _context.Tasks.Remove(task);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Projects");
        }

        private bool TasksExists(int id)
        {
            return (_context.Tasks?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
