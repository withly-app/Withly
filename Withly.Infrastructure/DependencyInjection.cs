using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Withly.Application.Auth.Interfaces;
using Withly.Application.Common.Interfaces;
using Withly.Domain.Repositories;
using Withly.Infrastructure.Auth;
using Withly.Infrastructure.Auth.Repositories;
using Withly.Infrastructure.Common.Services;
using Withly.Infrastructure.Email;
using Withly.Infrastructure.Events;
using Withly.Infrastructure.Models.Email;
using Withly.Infrastructure.Models.Email.Interfaces;
using Withly.Infrastructure.UserProfiles;
using Withly.Persistence;

namespace Withly.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(5), null);
                    npgsqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name);
                }));
        
        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();
        
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAuthTokenGenerator, JwtAuthTokenGenerator>();
        services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IAttachmentFactory, AttachmentFactory>();
        
        services.Configure<SmtpSettings>(configuration.GetRequiredSection("Smtp"));
        services.AddSingleton<IEmailTemplateRenderer, RazorTemplateRenderer>();
        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddSingleton<IBackgroundEmailQueue, BackgroundEmailQueue>();
        services.AddScoped<EmailMessageRepository>();
        services.AddHostedService<EmailBackgroundWorker>();
        services.AddHostedService<EmailDispatchWorker>();

        services.AddHostedService<DbMigrationService>();
        
        return services;
    }
    
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
    {
        var jwtSettings = config.GetRequiredSection("Jwt").Get<JwtSettings>()!;
        services.Configure<JwtSettings>(config.GetRequiredSection("Jwt"));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
                };
            });

        return services;
    }
}