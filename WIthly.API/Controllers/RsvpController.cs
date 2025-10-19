using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Withly.Domain.Enums;
using Withly.Persistence;

namespace Withly.API.Controllers;

[ApiController]
[Route("[controller]")]
[AllowAnonymous]
public class RsvpController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Respond(Guid eventId, Guid inviteeId, string secret, RsvpStatus response)
    {
        var rsvp = await db.Rsvps.FirstOrDefaultAsync(r => r.InviteeId == inviteeId && r.EventId == eventId);

        if (rsvp == null)
        {
            return NotFound();
        }

        if (rsvp.Secret != secret)
        {
            return Unauthorized();
        }

        rsvp.UpdateStatus(response);
        
        await db.SaveChangesAsync();
        return Ok();
    }
}