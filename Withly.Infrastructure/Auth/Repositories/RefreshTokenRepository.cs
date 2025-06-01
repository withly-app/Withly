using Microsoft.EntityFrameworkCore;
using Withly.Application.Auth.Interfaces;
using Withly.Persistence;

namespace Withly.Infrastructure.Auth.Repositories;

public class RefreshTokenRepository(AppDbContext dbContext) : IRefreshTokenRepository
{
    public async Task AddAsync(RefreshToken token, CancellationToken ct = default)
    {
        await dbContext.RefreshTokens.AddAsync(token, ct);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default)
    {
        return await dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == token, ct);
    }

    public void RemoveTokensForUser(Guid userId)
    {
        var tokens = dbContext.RefreshTokens.Where(rft => rft.UserId == userId);
        dbContext.RefreshTokens.RemoveRange(tokens);
    }
}