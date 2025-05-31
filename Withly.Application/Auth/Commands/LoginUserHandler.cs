using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Withly.Application.Auth.Interfaces;
using Withly.Persistence;

namespace Withly.Application.Auth.Commands;

[UsedImplicitly]
public class LoginUserHandler(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    IAuthTokenGenerator tokenGenerator)
    : IRequestHandler<LoginUserCommand, string>
{
    public async Task<string> Handle(LoginUserCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null) throw new Exception("Invalid credentials");

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded) throw new Exception("Invalid credentials");

        return tokenGenerator.Generate(user);
    }
}
