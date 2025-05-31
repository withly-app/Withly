using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Withly.Application.Auth.Dtos;
using Withly.Application.Auth.Interfaces;
using Withly.Application.Common;
using Withly.Infrastructure.Auth;
using Withly.Persistence;

namespace Withly.Application.Auth.Commands;

[UsedImplicitly]
public class LoginUserHandler(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    IAuthTokenGenerator tokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator,
    AppDbContext dbContext)
    : IRequestHandler<LoginUserCommand, Result<AuthResultDto>>
{
    public async Task<Result<AuthResultDto>> Handle(LoginUserCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null) return Result<AuthResultDto>.Fail("Invalid credentials");

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded) return Result<AuthResultDto>.Fail("Invalid credentials");

        var refreshToken = refreshTokenGenerator.Generate(user.Id); 

        dbContext.RefreshTokens.Add(refreshToken);
        await dbContext.SaveChangesAsync(ct);

        return Result<AuthResultDto>.Success(new AuthResultDto
        {
            AccessToken = tokenGenerator.Generate(user),
            RefreshToken = refreshToken.Token
        });
    }
}
