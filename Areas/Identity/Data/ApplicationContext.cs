using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Permissions;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Areas.Identity.Data;

public class ApplicationContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
//builder.Entity<ApplicationUser>().HasMany<Projects>().WithOne(p => p.ProjectManager).HasForeignKey(p => p.ProjectManagerId).OnDelete(DeleteBehavior.Cascade);
    }

    public DbSet<Models.Tasks> Tasks { get; set; } = default!;
    public DbSet<Projects> Projects { get; set; } = default!;
}
