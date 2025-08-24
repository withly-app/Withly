using Withly.Application.Events.Dtos;

namespace Withly.Application.Events;

public interface IEventFetcher
{
    Task<EventDetailsDto?> GetByIdAsync(Guid eventId, CancellationToken ct);
}