using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManagementSystem.Areas.Identity.Data;

namespace TaskManagementSystem.Models.ViewModels;

public class AssignRoleVM
{
    public IEnumerable<ApplicationUser> Users { get; set; } = null!;
    public IEnumerable<SelectListItem> Roles { get; set; } = null!;

    public string Role { get; set; } = null!;
}
