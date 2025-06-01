using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Withly.Application.Common.Interfaces;

namespace Withly.Infrastructure.Common.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;
    
    public Guid? UserId => Guid.TryParse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : null;

    public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value;
}