using Withly.Application.Events.Dtos;
using Withly.Domain.Repositories;

namespace Withly.Application.Events;

internal class EventFetcher(
    IEventRepository repository) : IEventFetcher
{
    public async Task<EventDetailsDto?> GetByIdAsync(Guid eventId, CancellationToken ct)
    {
        var @event = await repository.GetByIdWithInviteesAsync(eventId, ct);
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