using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace TaskManagementSystem.Areas.Identity.Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        ApplicationContext context = new(serviceProvider.GetRequiredService<DbContextOptions<ApplicationContext>>());
        UserManager<ApplicationUser> userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // === SEED ROLES
        List<string> roles = new() { "Administrator", "Project Manager", "Developer" };
        if (!context.Roles.Any(r => r.Name.Equals("Administrator")))
        {
            foreach (string s in roles)
            {
                await roleManager.CreateAsync(new IdentityRole(s));
            }
        }

        // === SEED ADMINISTRATOR
        if (!context.Users.Any(u => u.UserName == "Admin@test.ca"))
        {
            ApplicationUser seededAdmin = new()
            {
                Email = "Admin@test.ca",
                NormalizedEmail = "ADMIN@TEST.CA",
                UserName = "Admin@test.ca",
                NormalizedUserName = "ADMIN@TEST.CA",
                EmailConfirmed = true,
            };

            PasswordHasher<ApplicationUser> hasher = new();
            string hashedPw = hasher.HashPassword(seededAdmin, "TestP@ssword1");
            seededAdmin.PasswordHash = hashedPw;

            await userManager.CreateAsync(seededAdmin);
            await userManager.AddToRoleAsync(seededAdmin, "Administrator");
        }

        // === SEED Developer
        if (!context.Users.Any(u => u.UserName == "Devtest@.ca"))
        {
            ApplicationUser seededDev = new()
            {
                Email = "Dev@test.ca",
                NormalizedEmail = "DEV@TEST.CA",
                UserName = "Dev@test.ca",
                NormalizedUserName = "DEV@TEST.CA",
                EmailConfirmed = true,
            };

            PasswordHasher<ApplicationUser> hasher = new PasswordHasher<ApplicationUser>();
            string hashedPw = hasher.HashPassword(seededDev, "TestP@ssword1");
            seededDev.PasswordHash = hashedPw;

            await userManager.CreateAsync(seededDev);
            await userManager.AddToRoleAsync(seededDev, "Developer");
        }


        await context.SaveChangesAsync();
    }
}
