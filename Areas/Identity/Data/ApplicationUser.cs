using Microsoft.AspNetCore.Identity;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Areas.Identity.Data;

public class ApplicationUser : IdentityUser
{
    public virtual HashSet<ProjectDeveloper> Projects { get; } = new();
    public virtual HashSet<TaskDeveloper> Tasks { get; } = new();
}