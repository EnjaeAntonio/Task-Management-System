using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Models
{
    public class ApplicationTask
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 5)]
        public string Title { get; set; } = null!;

        [Required]
        [Range(1,999)]
        [Display(Name = "Required Hours")]
        public int RequiredHours { get; set; }
        public bool Completed { get; set; }
        public Priority Priority { get; set; }

        public int ApplicationProjectId { get; set; }
        public virtual ApplicationProject? Project { get; set; } = null!;

        public virtual HashSet<TaskDeveloper> Developers { get; } = new();
    }

    public enum Priority { Low, Medium, High }
}
