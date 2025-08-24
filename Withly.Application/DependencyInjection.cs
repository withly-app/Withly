using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using Withly.Application.Common.Behaviors;
using Withly.Application.Events;
using Withly.Application.Services;

namespace Withly.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddScoped<IEventCreator, EventCreator>();
        services.AddScoped<IEventFetcher, EventFetcher>();
        services.AddScoped<IEventMailer, EventMailer>();
        services.AddScoped<UserService>();
        services.AddScoped<RefreshTokenService>();
        services.AddScoped(typeof(RequestValidator<>));
        services.AddValidatorsFromAssembly(assembly);
        
        return services;
    }
}