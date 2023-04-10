using Microsoft.Build.Evaluation;
using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Areas.Identity.Data;

namespace TaskManagementSystem.Models
{
    public class Tasks
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(200, MinimumLength = 5)]
        public string Title { get; set; } = default!;

        [Required]
        [Display(Name = "Required Hours")]
        [Range(1,999)]
        public int RequiredHours { get; set; }
        public bool Completed { get; set; }
        public Priority Priority { get; set; }

        public Projects? Project { get; set; }
        public int ProjectId { get; set; }
        public HashSet<ApplicationUser> Developers { get; set; } = new HashSet<ApplicationUser>();

    }

    public enum Priority { Low, Medium, High }
}
