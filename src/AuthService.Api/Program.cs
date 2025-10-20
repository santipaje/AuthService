using AuthService.Infrastructure.Identity;
using AuthService.Infrastructure.Identity.Seed;
using AuthService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Logging configuration
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Infrastrucute and DB configuration
builder.Services.AddInfrastructure(builder.Configuration);

// TODO:
// Adds applications services
//builder.Services.AddApplicationServices();

// Controllers
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// App Builder
var app = builder.Build();

// Migrations and data seeds
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // Gets logger factory
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger("Seeder");

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
            await DefaultRolesSeeder.SeedAsync(roleManager, logger);
            await DefaultAdminSeeder.SeedAsync(userManager, logger);

            logger.LogInformation("Database migrated and seeded successfully.");

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while migrating or seeding the database.");
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
