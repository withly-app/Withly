using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Withly.Application.Auth.Dtos;
using Withly.Application.Auth.Interfaces;
using Withly.Application.Common;
using Withly.Persistence;

namespace Withly.Application.Auth.Commands;

public class ResetPasswordCommandHandler(
    UserManager<ApplicationUser> userManager,
    AppDbContext dbContext,
    IAuthTokenGenerator tokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator,
    ILogger<ResetPasswordCommandHandler> logger) : IRequestHandler<ResetPasswordCommand, Result<AuthResultDto>>
{
    public async Task<Result<AuthResultDto>> Handle(ResetPasswordCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            logger.LogWarning("User not found, probably forged request");
            return Result<AuthResultDto>.Fail("User not found");
        }

        var result = await userManager.ResetPasswordAsync(user, request.Token, request.Password);
        if (!result.Succeeded)
            return Result<AuthResultDto>.Fail(string.Join("; ", result.Errors.Select(e => e.Description)));

        var tokens = dbContext.RefreshTokens.Where(rft => rft.UserId == user.Id);
        dbContext.RefreshTokens.RemoveRange(tokens);
        
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
