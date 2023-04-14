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
    [Authorize(Roles = "Developer")]
    public class TasksController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<TasksController> _logger;


        public TasksController(ApplicationContext context, UserManager<ApplicationUser> userManager, ILogger<TasksController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Tasks
        public async Task<IActionResult> Index(int? ProjectId, string OrderBy, string OrderDirection, bool? ShowCompleted, bool? ShowAssigned)
        {
            string currentUserId = _userManager.GetUserId(User);

                var tasks = await _context.Tasks
                  .Where(t => t.Developers.Any(td => td.User.UserName == User.Identity.Name))
                  .Include(t => t.Project)
                  .ToListAsync();

   
            if (ProjectId.HasValue)
            {
                tasks = tasks
                    .Where(t => t.ApplicationProjectId == ProjectId.Value)
                    .ToList();
            }

            if (!ShowCompleted.GetValueOrDefault())
            {
                tasks = tasks
                    .Where(t => !t.Completed)
                    .ToList();
            }

            if (!ShowAssigned.GetValueOrDefault())
            {
                tasks = tasks
                    .Where(t => !t.Developers.Any())
                    .ToList();
            }

            switch (OrderBy)
            {
                case "RequiredHours":
                    if (OrderDirection == "Ascending")
                    {
                        tasks = tasks.OrderBy(t => t.RequiredHours).ToList();
                    }
                    else
                    {
                        tasks = tasks.OrderByDescending(t => t.RequiredHours).ToList();
                    }
                    break;
                case "Priority":
                    if (OrderDirection == "Ascending")
                    {
                        tasks = tasks.OrderBy(t => t.Priority).ToList();
                    }
                    else
                    {
                        tasks = tasks.OrderByDescending(t => t.Priority).ToList();
                    }
                    break;
            }

            return View(tasks);
        }


        public async Task<IActionResult> Assign(int? taskId, int? projectId)
        {
            if (taskId == null || projectId == null)
            {
                return NotFound();
            }

            var task = _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefault(p => p.Id == taskId);
            var project = _context.Projects.Find(projectId);

            if (task == null || project == null)
            {
                return NotFound();
            }

            var developers = await _userManager.GetUsersInRoleAsync("Developer");

            AssignTaskViewModel viewModel = new AssignTaskViewModel()
            {
                Project = project,
                Task = task,
                Developers = developers,
            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Assign(int? taskId, int? projectId, AssignTaskViewModel viewModel)
        {
            var project = await _context.Projects.FindAsync(projectId);
            var task = await _context.Tasks.FindAsync(taskId);

            if (task == null)
            {
                return NotFound();
            }
            var taskDeveloper = new TaskDeveloper
            {
                ApplicationTaskId = task.Id,
                ApplicationUserId = viewModel.SelectedDeveloperId
            };

            var projectDeveloper = new ProjectDeveloper
            {
                ApplicationUserId = viewModel.SelectedDeveloperId,
                ApplicationProjectId = project.Id
            };

            _context.TaskDevelopers.Add(taskDeveloper);
            _context.ProjectDevelopers.Add(projectDeveloper);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tasks == null)
            {
                return NotFound();
            }

            return View(tasks);
        }

        // GET: Tasks/Create
        public IActionResult Create(int? Id)
        {

            if (Id == null)
            {
                return BadRequest();
            }

            ApplicationProject project = _context.Projects.FirstOrDefault(p => p.Id == Id);
            ViewBag.ProjectId = project.Id;

            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int Id, [Bind("Title,RequiredHours,Completed,Priority,ApplicationProjectId")] ApplicationTask tasks)
        {
            ApplicationProject project = _context.Projects.FirstOrDefault(p => p.Id == Id);
            ViewBag.ProjectId = project.Id;

            tasks.Project = project;
            tasks.ApplicationProjectId = project.Id;

            if (ModelState.IsValid)
            {
                project.Tasks.Add(tasks);
                _context.Add(tasks);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Projects");
            }

            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Title", tasks.ApplicationProjectId);
            return RedirectToAction("Index", "Projects");
        }


        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks.FindAsync(id);
            if (tasks == null)
            {
                return NotFound();
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Title", tasks.ApplicationProjectId);
            return View(tasks);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,RequiredHours,Completed,Priority,ApplicationProjectId")] ApplicationTask tasks)
        {
            if (id != tasks.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tasks);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TasksExists(tasks.Id))
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
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Title", tasks.ApplicationProjectId);
            return View(tasks);
        }

        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tasks == null)
            {
                return NotFound();
            }

            return View(tasks);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Tasks == null)
            {
                return Problem("Entity set 'ApplicationContext.Tasks'  is null.");
            }
            var tasks = await _context.Tasks.FindAsync(id);
            if (tasks != null)
            {
                _context.Tasks.Remove(tasks);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TasksExists(int id)
        {
          return (_context.Tasks?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
