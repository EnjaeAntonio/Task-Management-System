using Microsoft.AspNetCore.Identity;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Areas.Identity.Data;

public class ApplicationUser : IdentityUser
{
    public HashSet<UserProjects> UserProjects { get; set; }
    public HashSet<Projects> Projects { get; set; } = new HashSet<Projects>();
}

