using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Areas.Identity.Data;

namespace TaskManagementSystem.Models
{
    public class UserProjects
    {
        public int Id { get; set; }

        [Display(Name = "Project Manager")]
        public ApplicationUser ProjectManager { get; set; }
        public string ApplicationUserId { get; set; }

        public Projects Project { get; set; }
        public int ProjectId { get; set; }
    }
}

