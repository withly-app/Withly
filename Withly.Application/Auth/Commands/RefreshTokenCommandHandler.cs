using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Withly.Application.Auth.Interfaces;
using Withly.Application.Auth.Dtos;
using Withly.Application.Common;
using Withly.Application.Common.Interfaces;
using Withly.Persistence;

namespace Withly.Application.Auth.Commands;

[UsedImplicitly]
public class RefreshTokenHandler(
    UserManager<ApplicationUser> userManager,
    IUnitOfWork unitOfWork,
    IAuthTokenGenerator authTokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator,
    IRefreshTokenRepository refreshTokenRepository)
    : IRequestHandler<RefreshTokenCommand, Result<AuthResultDto>>
{
    public async Task<Result<AuthResultDto>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var token = await refreshTokenRepository.GetByTokenAsync(request.Token, ct);

        if (token == null || token.IsExpired || token.IsRevoked)
        {
            return Result<AuthResultDto>.Fail("Invalid or expired refresh token");
        }

        var user = await userManager.FindByIdAsync(token.UserId.ToString());
        if (user == null)
        {
            return Result<AuthResultDto>.Fail("User not found");
        }
        
        token.Revoke();
        var newRefreshToken = refreshTokenGenerator.Generate(user.Id);

        await refreshTokenRepository.AddAsync(newRefreshToken, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result<AuthResultDto>.Success(new AuthResultDto
        {
            AccessToken = authTokenGenerator.Generate(user),
            RefreshToken = newRefreshToken.Token
        });
    }
}
