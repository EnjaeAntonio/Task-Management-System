using TaskManagementSystem.Areas.Identity.Data;

namespace TaskManagementSystem.Models
{
    public class TaskDeveloper
    {
        public int Id { get; set; }

        public int ApplicationTaskId { get; set; }
        public virtual ApplicationTask Task { get; set; } = null!;

        public string ApplicationUserId { get; set; } = null!;
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
