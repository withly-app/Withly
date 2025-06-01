using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Withly.Application.Events.Commands;
using Withly.Application.Events.Queries;

namespace Withly.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventsController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.Send(new GetEventByIdQuery(id));
        if (result is null)
            return NotFound();

        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEventCommand command)
    {
        var id = await mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

}