using AuthService.Infrastructure.Identity;
using AuthService.Infrastructure.Identity.Seed;
using AuthService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Data.Common;

namespace AuthService.IntegrationTests.Common
{
    /// <summary>
    /// Custom factory for creating and configuring a test host (WebApplicationFactory) used for <b>Integration Tests</b>.
    /// This class is used to override services (e.g. swapping SQL Server DbContext with an in-memory SQLite provider) adn set up a controlled environment for API testing.
    /// </summary>
    /// <typeparam name="TProgram"></typeparam>
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        private SqliteConnection _connection = default!;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("IntegrationTests");

            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext options
                var dbContextOptionsDescriptor = services.SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<AuthDbContext>));
                if (dbContextOptionsDescriptor != null) { services.Remove(dbContextOptionsDescriptor); }

                // Remove the existing DbConnection if exists
                var dbConnectionDescriptor = services.SingleOrDefault(service => service.ServiceType == typeof(DbConnection));
                if (dbConnectionDescriptor != null) { services.Remove(dbConnectionDescriptor); }

                // Create and keep SQLite in-memory connection
                _connection = new SqliteConnection("DataSource=:memory:");
                _connection.Open();

                services.AddSingleton<DbConnection>(_connection);

                services.AddDbContext<AuthDbContext>((container, options) =>
                {
                    var connection = container.GetRequiredService<DbConnection>();
                    options.UseSqlite(connection);
                });


                // Build Service provider and apply migrations and seed
                var sp = services.BuildServiceProvider();

                // Create scope and get DbContext
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

                // Open connection
                db.Database.OpenConnection();

                // Apply migrations
                db.Database.EnsureCreated();

                // Identity managers
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                // Execute seeders
                _ = DefaultRolesSeeder.SeedAsync(roleManager);
                _ = DefaultAdminSeeder.SeedAsync(userManager);
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _connection?.Dispose();
        }

    }
}
