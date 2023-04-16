using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Areas.Identity.Data;

namespace TaskManagementSystem.Models
{
    public class ApplicationProject
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 5)]
        public string Title { get; set; } = null!;

        public string ApplicationUserId { get; set; } = null!;
        [Display(Name = "Project Manager")]
        public virtual ApplicationUser? ProjectManager { get; set; } = null!;

        public virtual HashSet<ApplicationTask> Tasks { get; set; } = new();
        public virtual HashSet<ProjectDeveloper> Developers { get; set; } = new();
    }
}
