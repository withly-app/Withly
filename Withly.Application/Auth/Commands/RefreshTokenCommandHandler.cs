using MediatR;
using Microsoft.EntityFrameworkCore;
using Withly.Application.Auth.Interfaces;
using Withly.Application.Auth.Dtos;
using Withly.Persistence;

namespace Withly.Application.Auth.Commands;

public class RefreshTokenHandler(
    AppDbContext dbContext,
    IAuthTokenGenerator authTokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator)
    : IRequestHandler<RefreshTokenCommand, AuthResultDto>
{
    public async Task<AuthResultDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var token = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == request.Token, cancellationToken);

        if (token == null || token.IsExpired || token.IsRevoked)
        {
            throw new Exception("Invalid or expired refresh token");
        }

        var user = await dbContext.Users.FindAsync([token.UserId], cancellationToken);
        if (user == null)
        {
            throw new Exception("User not found");
        }
        
        token.Revoke();
        var newRefreshToken = refreshTokenGenerator.Generate(user.Id);

        dbContext.RefreshTokens.Add(newRefreshToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new AuthResultDto
        {
            AccessToken = authTokenGenerator.Generate(user),
            RefreshToken = newRefreshToken.Token
        };
    }
}
