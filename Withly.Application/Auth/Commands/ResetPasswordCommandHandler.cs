using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Withly.Application.Auth.Dtos;
using Withly.Application.Auth.Interfaces;
using Withly.Application.Common;
using Withly.Application.Common.Interfaces;
using Withly.Persistence;

namespace Withly.Application.Auth.Commands;

public class ResetPasswordCommandHandler(
    UserManager<ApplicationUser> userManager,
    IUnitOfWork unitOfWork,
    IAuthTokenGenerator tokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator,
    IRefreshTokenRepository refreshTokenRepository,
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

        refreshTokenRepository.RemoveTokensForUser(user.Id);
        
        var refreshToken = refreshTokenGenerator.Generate(user.Id);

        await refreshTokenRepository.AddAsync(refreshToken, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result<AuthResultDto>.Success(new AuthResultDto
        {
            AccessToken = tokenGenerator.Generate(user),
            RefreshToken = refreshToken.Token
        });
    }
}
