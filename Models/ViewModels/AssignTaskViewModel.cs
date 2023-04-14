using TaskManagementSystem.Areas.Identity.Data;

namespace TaskManagementSystem.Models.ViewModels
{
    public class AssignTaskViewModel
    {
        public ApplicationTask Task { get; set; }
        public List<ApplicationUser> Developers { get; set; }
        public string SelectedDeveloperId { get; set; }
    }
}
