using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Withly.Application.Auth.Interfaces;
using Withly.Application.Auth.Dtos;
using Withly.Application.Common;
using Withly.Persistence;

namespace Withly.Application.Auth.Commands;

[UsedImplicitly]
public class RefreshTokenHandler(
    AppDbContext dbContext,
    IAuthTokenGenerator authTokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator)
    : IRequestHandler<RefreshTokenCommand, Result<AuthResultDto>>
{
    public async Task<Result<AuthResultDto>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var token = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == request.Token, ct);

        if (token == null || token.IsExpired || token.IsRevoked)
        {
            return Result<AuthResultDto>.Fail("Invalid or expired refresh token");
        }

        var user = await dbContext.Users.FindAsync([token.UserId], ct);
        if (user == null)
        {
            return Result<AuthResultDto>.Fail("User not found");
        }
        
        token.Revoke();
        var newRefreshToken = refreshTokenGenerator.Generate(user.Id);

        dbContext.RefreshTokens.Add(newRefreshToken);
        await dbContext.SaveChangesAsync(ct);

        return Result<AuthResultDto>.Success(new AuthResultDto
        {
            AccessToken = authTokenGenerator.Generate(user),
            RefreshToken = newRefreshToken.Token
        });
    }
}
