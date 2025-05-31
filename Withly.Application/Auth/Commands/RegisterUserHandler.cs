using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Withly.Application.Auth.Exceptions;
using Withly.Application.Auth.Interfaces;
using Withly.Persistence;

namespace Withly.Application.Auth.Commands;

[UsedImplicitly]
public class RegisterUserHandler(UserManager<ApplicationUser> userManager, IAuthTokenGenerator tokenGenerator)
    : IRequestHandler<RegisterUserCommand, string>
{
    public async Task<string> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        if (await userManager.FindByEmailAsync(request.Email) is not null)
            throw new UserAlreadyExistsException("Email is already registered.");
        
        var user = new ApplicationUser { Email = request.Email, UserName = request.Email };
        var result = await userManager.CreateAsync(user, request.Password);

        if (result.Succeeded) return tokenGenerator.Generate(user);
        
        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        throw new Exception($"Registration failed: {errors}");
    }
}