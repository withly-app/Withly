using Microsoft.AspNetCore.Identity;
using Withly.Application.Auth.Dtos;
using Withly.Application.Auth.Interfaces;
using Withly.Application.Common;
using Withly.Application.Common.Interfaces;
using Withly.Persistence;

namespace Withly.Application.Services;

public class RefreshTokenService(UserManager<ApplicationUser> userManager,
    IUnitOfWork unitOfWork,
    IAuthTokenGenerator authTokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator,
    IRefreshTokenRepository refreshTokenRepository)
{
    public async Task<Result<AuthResultDto>> GetAuthTokenAsync(string refreshToken, CancellationToken ct)
    {
        var token = await refreshTokenRepository.GetByTokenAsync(refreshToken, ct);

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