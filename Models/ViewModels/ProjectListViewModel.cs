using System.ComponentModel;

namespace TaskManagementSystem.Models.ViewModels
{
    public class ProjectListViewModel
    {
        public TaskOrderBy OrderBy { get; set; }
        public ListSortDirection OrderDirection { get; set; }
        public bool ShowCompleted { get; set; }
        public bool ShowAssigned { get; set; }

        public IEnumerable<ApplicationProject> Projects { get; set; } = null!;
    }

    public enum TaskOrderBy
    {
        RequiredHours,
        Priority
    }
}
