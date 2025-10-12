using Withly.Persistence.Entities;

namespace Withly.Application.Auth.Interfaces;

public interface IRefreshTokenGenerator
{
    public RefreshToken Generate(Guid userId);
}