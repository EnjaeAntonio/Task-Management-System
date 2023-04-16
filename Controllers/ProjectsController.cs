using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Transactions;
using TaskManagementSystem.Areas.Identity.Data;
using TaskManagementSystem.Models;
using TaskManagementSystem.Models.ViewModels;

namespace TaskManagementSystem.Controllers
{
    [Authorize(Roles = "Project Manager, Developer")]
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
        public async Task<IActionResult> Index(
            TaskOrderBy orderBy,
            ListSortDirection orderDirection,
            bool showCompleted = true,
            bool showAssigned = true,
            int pageNumber = 1)
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(User);
            IList<string> roles = await _userManager.GetRolesAsync(currentUser);

            int pageSize = 3;

            ProjectListViewModel vm = new()
            {
                OrderBy = orderBy,
                OrderDirection = orderDirection,
                ShowCompleted = showCompleted,
                ShowAssigned = showAssigned,
                CurrentPage = pageNumber,
            };

            if (await _userManager.IsInRoleAsync(currentUser, "Developer"))
            {
                vm.Projects = _context.Projects
                    .Where(p => p.Developers.Any(pd => pd.ApplicationUserId == currentUser.Id))
                    .Include(p => p.Tasks.Where(t => t.Developers.Any(td => td.ApplicationUserId == currentUser.Id)))
                    .AsEnumerable();
            }
            else
            {
                vm.Projects = _context.Projects
                    .Where(p => p.ApplicationUserId == currentUser.Id)
                    .Include(t => t.Tasks)
                    .ThenInclude(t => t.Developers)
                    .AsEnumerable();
            }

            foreach (ApplicationProject project in vm.Projects)
            {
                IEnumerable<ApplicationTask> tasks = project.Tasks;

                if (!showCompleted)
                {
                    tasks = tasks.Where(t => !t.Completed);
                }

                if (!showAssigned)
                {
                    tasks = tasks.Where(t => !t.Developers.Any());
                }

                switch (orderBy)
                {
                    case TaskOrderBy.RequiredHours:
                        if (orderDirection == ListSortDirection.Ascending)
                        {
                            tasks = tasks.OrderBy(t => t.RequiredHours);
                        }
                        else
                        {
                            tasks = tasks.OrderByDescending(t => t.RequiredHours);
                        }
                        break;
                    case TaskOrderBy.Priority:
                        if (orderDirection == ListSortDirection.Ascending)
                        {
                            tasks = tasks.OrderBy(t => t.Priority);
                        }
                        else
                        {
                            tasks = tasks.OrderByDescending(t => t.Priority);
                        }
                        break;
                };

                project.Tasks = tasks.ToHashSet();
            }

            vm.Projects = vm.Projects.OrderBy(p => p.Title, StringComparer.OrdinalIgnoreCase);

            int totalProjects = vm.Projects.Count();
            int totalPages = (int)Math.Ceiling((decimal)totalProjects / pageSize);
            int skip = (pageNumber - 1) * pageSize;
            vm.Projects = vm.Projects.Skip(skip).Take(pageSize);

            vm.Pagination = new PaginationViewModel()
            {
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };

            return View(vm);
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ApplicationProject? projects = await _context.Projects.FindAsync(id);

            if (projects == null)
            {
                return NotFound();
            }

            return View(projects);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Project Manager")]
        public async Task<IActionResult> Create()
        {
            ApplicationUser projectManager = await _userManager.GetUserAsync(User);
            IList<ApplicationUser> developers = await _userManager.GetUsersInRoleAsync("Developer");

            CreateProjectViewModel vm = new()
            {
                Developers = developers,
                ApplicationUserId = projectManager.Id
            };

            return View(vm);
        }

        // POST: Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Project Manager")]
        public async Task<IActionResult> Create(CreateProjectViewModel CreateProject)
        {
            HashSet<ProjectDeveloper> projectDevelopers = new();

            ApplicationProject project = new()
            {
                Title = CreateProject.Title,
                ApplicationUserId = CreateProject.ApplicationUserId
            };

            foreach (string userId in CreateProject.SelectedDevelopersIdList)
            {
                ProjectDeveloper projectDeveloper = new()
                {
                    ApplicationUserId =  userId,
                    ApplicationProjectId = project.Id,
                };

                project.Developers.Add(projectDeveloper);
            }

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ApplicationProject? project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title")] ApplicationProject project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectsExists(project.Id))
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
            return View(project);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "Project Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ApplicationProject? project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Project Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ApplicationProject? projects = await _context.Projects.FindAsync(id);

            if (projects != null)
            {
                _context.Projects.Remove(projects);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectsExists(int id)
        {
          return (_context.Projects?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
