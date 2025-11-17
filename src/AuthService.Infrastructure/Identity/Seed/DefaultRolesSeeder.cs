using Microsoft.AspNetCore.Identity;
using Serilog;

namespace AuthService.Infrastructure.Identity.Seed
{
    /// <summary>
    /// Default seeder to create the indicated roles if they do not exist.
    /// </summary>
    public static class DefaultRolesSeeder
    {
        private static readonly string[] Roles = ["Admin", "User"];

        /// <summary>
        /// Creates the roles if they do not exist.
        /// </summary>
        /// <param name="roleManager"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    Log.Information("Role '{rolen}' created successfully", role);
                }
            }
        }

    }
}
