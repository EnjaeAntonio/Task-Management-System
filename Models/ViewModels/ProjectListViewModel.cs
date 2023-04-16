using System.ComponentModel;

namespace TaskManagementSystem.Models.ViewModels
{
    public class ProjectListViewModel
    {
        public TaskOrderBy OrderBy { get; set; }
        public ListSortDirection OrderDirection { get; set; }
        public bool ShowCompleted { get; set; }
        public bool ShowAssigned { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public PaginationViewModel Pagination { get; set; } = new PaginationViewModel();

        public IEnumerable<ApplicationProject> Projects { get; set; } = null!;
    }

    public enum TaskOrderBy
    {
        RequiredHours,
        Priority
    }
}
