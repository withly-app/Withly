using Withly.Infrastructure.Auth;

namespace Withly.Application.Auth.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token, CancellationToken ct = default);

    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default);
    void RemoveTokensForUser(Guid userId);
}