using System.Security.Cryptography;
using Withly.Application.Auth.Interfaces;

namespace Withly.Infrastructure.Auth;

public class RefreshTokenGenerator : IRefreshTokenGenerator
{
    public RefreshToken Generate(Guid userId)
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        var token = Convert.ToBase64String(bytes);
        return new RefreshToken(token, userId);
    }
}