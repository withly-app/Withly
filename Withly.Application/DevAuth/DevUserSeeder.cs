using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Withly.Application.Auth.Dtos;
using Withly.Application.Auth.Interfaces;
using Withly.Application.Services;
using Withly.Persistence;

namespace Withly.Application.DevAuth;

public sealed class DevUserSeeder(
    ILogger<DevUserSeeder> logger,
    IServiceScopeFactory serviceScopeFactory,
    IHostEnvironment env,
    IOptions<DevAuthOptions> options,
    DevTokenProvider provider) : IHostedService
{
    public async Task StartAsync(CancellationToken ct)
    {
        if (!env.IsDevelopment() || !options.Value.Enabled) return;

        using var scope = serviceScopeFactory.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<UserService>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var authTokenGen = scope.ServiceProvider.GetRequiredService<IAuthTokenGenerator>();

        var email = options.Value.User.Email.ToLowerInvariant();

        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            var result = await userService.RegisterAsync(new RegisterUserDto
            {
                Email = email,
                Password = options.Value.User.Password,
                DisplayName = options.Value.User.Name
            }, ct);

            if (!result.IsSuccess)
            {
                logger.LogError("Failed to create dev user: {Error}", result.Error);
                return;
            }

            provider.Token = result.Value!.AccessToken;
            return;
        }


        provider.Token = authTokenGen.Generate(user);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}