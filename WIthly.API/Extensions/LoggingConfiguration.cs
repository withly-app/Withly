using Serilog;

namespace Withly.API.Extensions;

public static class LoggingConfiguration
{
    public static void AddSerilog(this ConfigureHostBuilder hostBuilder, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .CreateLogger();
        
        hostBuilder.UseSerilog();
    }
}