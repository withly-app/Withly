using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Withly.Application.UserProfiles.Queries;

namespace Withly.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserProfilesController(IMediator mediator) : Controller
{
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetById(Guid userId)
    {
        var profile = await mediator.Send(new GetUserProfileQuery(userId));
        if (profile is null) 
            return NotFound();
        
        return Ok(profile);
    }
    
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUserProfile()
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdStr, out var userId))
            return Unauthorized();

        var profile = await mediator.Send(new GetUserProfileQuery(userId));
        if (profile is null)
            return NotFound();

        return Ok(profile);
    }
}