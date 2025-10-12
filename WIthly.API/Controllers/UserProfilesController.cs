using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Withly.Application.Services;

namespace Withly.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserProfilesController(UserService userService) : Controller
{
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetById(Guid userId, CancellationToken ct = default)
    {
        var profile = await userService.GetUserByIdAsync(userId, ct);
        
        return Ok(profile);
    }
    
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUserProfile(CancellationToken ct = default)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdStr, out var userId))
            return Unauthorized();

        var profile = await userService.GetUserByIdAsync(userId, ct);

        return Ok(profile);
    }
}