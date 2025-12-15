using AuthService.Infrastructure.Configuration;
using AuthService.Infrastructure.Identity;
using AuthService.Infrastructure.Identity.Seed;
using AuthService.Infrastructure.Logging;
using AuthService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.ConfigureSerilog(builder.Configuration);

// Add Infrastrucute and DB configuration
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);

// Add Services
builder.Services.AddApplicationServices();

// Add Validators
builder.Services.AddValidators();

// Controllers
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Log.Information("Starting up AuthService...");

// App Builder
var app = builder.Build();

try
{
    // Migrations and data seeds
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        if (app.Environment.IsDevelopment())
        {
            try
            {
                // Automatic database migration
                var context = services.GetRequiredService<AuthDbContext>();
                await context.Database.MigrateAsync();

                // Identity managers
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

                // Execute seeders
                await DefaultRolesSeeder.SeedAsync(roleManager);
                await DefaultAdminSeeder.SeedAsync(userManager);

                Log.Information("Database migrated and seeded successfully.");

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while migrating or seeding the database.");
                throw;
            }
        }

    }

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    // Enables authentication and authorization
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();

    Log.Information("AuthService stopped successfully.");
}
catch (Exception ex)
{
    Log.Fatal(ex, "AuthService terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }