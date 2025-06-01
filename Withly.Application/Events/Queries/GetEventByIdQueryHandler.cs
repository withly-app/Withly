using MediatR;
using Microsoft.EntityFrameworkCore;
using Withly.Application.Events.Dtos;
using Withly.Application.Events.Queries;
using Withly.Domain.Repositories;

namespace Withly.Application.Events.Queries;

public class GetEventByIdQueryHandler(
    IEventRepository repository)
    : IRequestHandler<GetEventByIdQuery, EventDetailsDto?>
{
    public async Task<EventDetailsDto?> Handle(GetEventByIdQuery request, CancellationToken ct)
    {
        var @event = await repository.GetByIdWithInviteesAsync(request.EventId, ct);
        if (@event is null) return null;

        return new EventDetailsDto(
            @event.Id,
            @event.Title,
            @event.Description,
            @event.StartUtc,
            @event.EndUtc,
            @event.IsRecurring,
            @event.RecurringRule,
            @event.IsPublic,
            @event.PublicJoinCode,
            @event.IsPublic
                ? [] // No invitees for public events
                : @event.Invitees
                    .Select(i => new InviteeDto(i.Email, i.Name, i.RsvpStatus.ToString()))
                    .ToList()
        );
    }
}