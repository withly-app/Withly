using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Withly.Application.Events;
using Withly.Application.Events.Dtos;

namespace Withly.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventsController(IEventFetcher eventFetcher, IEventCreator eventCreator) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var result = await eventFetcher.GetByIdAsync(id, ct);
        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateEventDto dto, CancellationToken ct = default)
    {
        try
        {
            var id = await eventCreator.CreateEventAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }
        catch (Exception)
        {
            return Problem("An error occurred while creating the event.");
        }
    }

}