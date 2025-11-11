using Microsoft.OpenApi;
using Withly.Application;
using Withly.Infrastructure;

namespace Withly.API.Extensions;

public static class ApiServiceCollectionExtensions
{
    public static IServiceCollection AddWithlyApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Withly API", Version = "v1" });

                var basePath = AppContext.BaseDirectory;

                foreach (var xmlFile in Directory.GetFiles(basePath, "*.xml", SearchOption.TopDirectoryOnly))
                    c.IncludeXmlComments(xmlFile, includeControllerXmlComments: true);

                c.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = "Bearer"
                    }
                );

                c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Bearer", doc)] = []
                    }
                );
            }
        );

        services.Configure<RouteOptions>(o => o.LowercaseUrls = true);

        services.AddApplication();
        services.AddInfrastructure(configuration);

        services.AddAppHealthChecks();
        services.AddControllers();

        services.AddJwtAuthentication(configuration);
        services.AddAuthorization();


        return services;
    }
}