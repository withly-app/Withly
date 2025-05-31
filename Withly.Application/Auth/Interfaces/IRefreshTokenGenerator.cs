using Withly.Infrastructure.Auth;

namespace Withly.Application.Auth.Interfaces;

public interface IRefreshTokenGenerator
{
    public RefreshToken Generate(Guid userId);
}