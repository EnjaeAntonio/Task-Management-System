using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Areas.Identity.Data;

namespace TaskManagementSystem.Models.ViewModels
{
    public class CreateProjectViewModel
    {
        [Required]
        [StringLength(200, MinimumLength = 5)]
        public string Title { get; set; } = null!;

        public string ApplicationUserId { get; set; } = null!;

        public IEnumerable<ApplicationUser> Developers { get; set; } = null!;
        public List<string> SelectedDevelopersIdList { get; set; } = null!;    
    }
}
