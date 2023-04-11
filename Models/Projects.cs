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
        public virtual HashSet<Tasks> Tasks { get; set; } = new HashSet<Tasks>();
        public virtual HashSet<ApplicationUser> Developers { get; set; } = new HashSet<ApplicationUser> { };
    }
}
