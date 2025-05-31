using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Withly.Application.Common.Interfaces;

namespace Withly.Infrastructure.Auth;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid? UserId
    {
        get
        {
            var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userId, out var guid) ? guid : null;
        }
    }

    public string? Email =>
        httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;
}