using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;
using Withly.Persistence;

namespace Withly.API.Extensions;

public static class HealthCheckExtensions
{
    public static void AddAppHealthChecks(this IServiceCollection services)
    { 
        services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>("PostgreSQL", tags: new[] { "db", "ready" });
    }

    public static IApplicationBuilder UseAppHealthChecks(this IApplicationBuilder app)
    {
        return app.UseHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var result = JsonSerializer.Serialize(new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(entry => new
                    {
                        name = entry.Key,
                        status = entry.Value.Status.ToString(),
                        error = entry.Value.Exception?.Message
                    })
                });
                await context.Response.WriteAsync(result);
            }
        });
    }
}