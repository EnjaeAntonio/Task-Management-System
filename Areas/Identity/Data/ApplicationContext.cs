using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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
        builder.Entity<ProjectDeveloper>()
            .HasOne(p => p.User)
            .WithMany(p => p.Projects)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<TaskDeveloper>()
            .HasOne(t => t.User)
            .WithMany(d => d.Tasks)
            .OnDelete(DeleteBehavior.NoAction);
    }

    public DbSet<ApplicationProject> Projects { get; set; } = default!;
    public DbSet<ApplicationTask> Tasks { get; set; } = default!;

    public DbSet<ProjectDeveloper> ProjectDevelopers { get; set; } = default!;
    public DbSet<TaskDeveloper> TaskDevelopers { get; set; } = default!;
}
