using Microsoft.AspNetCore.Identity;
using Serilog;

namespace AuthService.Infrastructure.Identity.Seed
{
    /// <summary>
    /// Default seeder to create the default admin user if it does not exist.
    /// </summary>
    public static class DefaultAdminSeeder
    {
        /// <summary>
        /// Creates default admin user if it does not exist.
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
        {
            var adminEmail = "admin@authservice.local";
            var adminPwd = "Admin1234!";

            var existingAdmin = await userManager.FindByNameAsync(adminEmail);
            if (existingAdmin != null)
            {
                Log.Information("Default admin user already exists");
                return;
            }

            var adminUser = new ApplicationUser { UserName = adminEmail, FullName = "Admin", Email = adminEmail, EmailConfirmed = true };

            var result = await userManager.CreateAsync(adminUser, adminPwd);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                Log.Information("Default admin user created successfully.");
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                Log.Error("Failed to create default admin user: {Errors}", errors);
            }
        }
    }
}
