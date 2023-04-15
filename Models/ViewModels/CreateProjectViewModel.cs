using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Areas.Identity.Data;

namespace TaskManagementSystem.Models.ViewModels
{
    public class CreateProjectViewModel
    {
        public string Title { get; set; } = null!;
        public string ApplicationUserId { get; set; } = null!;
        [Display(Name = "Project Manager")]
        public virtual ApplicationUser? ProjectManager { get; set; } = null!;
        public IEnumerable<ApplicationUser> Developers { get; set; } = null!;
        public List<string> SelectedDevelopersIdList { get; set; } = null!;
        
    }
}
