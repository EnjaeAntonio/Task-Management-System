using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Areas.Identity.Data;

namespace TaskManagementSystem.Models
{
    public class Projects
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(200, MinimumLength = 5)]
        public string Title { get; set; } = default!;
        public HashSet<UserProjects> UserProjects { get; set; }
        public HashSet<Tasks> Tasks { get; set; } = new HashSet<Tasks>();

        public HashSet<ApplicationUser> Developers { get; set; } = new HashSet<ApplicationUser> { };
    }
}
