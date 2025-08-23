using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Withly.Application.Events.Commands;
using Withly.Application.Services;

namespace Withly.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventsController(EventService eventService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var result = await eventService.GetByIdAsync(id, ct);
        if (result is null)
            return NotFound();

        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateEventObject @object, CancellationToken ct = default)
    {
        var id = await eventService.CreateEventAsync(@object, ct);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

}