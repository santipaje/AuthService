
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace AuthService.Infrastructure.Logging
{
    /// <summary>
    /// Serilog configuration
    /// </summary>
    public static class SerilogConfiguration
    {
        public static void ConfigureSerilog(this IHostBuilder hostBuilder, IConfiguration configuration)
        {
            // Creates looger with appsettings configuration
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            // Connects Serilog to Host
            hostBuilder.UseSerilog();
        }
    }
}
