using TaskManagementSystem.Areas.Identity.Data;

namespace TaskManagementSystem.Models
{
    public class ProjectDeveloper
    {
        public int Id { get; set; }

        public int ApplicationProjectId { get; set; }
        public virtual ApplicationProject Project { get; set; } = null!;

        public string ApplicationUserId { get; set; } = null!;
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
