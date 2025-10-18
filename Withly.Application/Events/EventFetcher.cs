using Microsoft.EntityFrameworkCore;
using Withly.Application.Events.Dtos;
using Withly.Persistence;
using Withly.Persistence.Entities;

namespace Withly.Application.Events;

internal class EventFetcher(
    AppDbContext dbContext) : IEventFetcher
{
    public async Task<EventDetailsDto?> GetByIdAsync(Guid eventId, CancellationToken ct)
    {
        return await dbContext.Events
            .AsNoTracking()
            .Where(e => e.Id == eventId)
            .Select<Event, EventDetailsDto>(e => new EventDetailsDto(
                e.Id,
                e.Title,
                e.Description,
                e.StartUtc,
                e.EndUtc,
                e.IsRecurring,
                e.RecurringRule,
                e.IsPublic,
                e.PublicJoinCode,
                e.Invitees.Where(_ => e.IsPublic)
                    .Select(i => new InviteeDto(i.Email, i.Name, i.Rsvps.First(r => r.EventId == e.Id).Status.ToString()))
                    .ToList()
            ))
            .FirstOrDefaultAsync(ct);
    }
}