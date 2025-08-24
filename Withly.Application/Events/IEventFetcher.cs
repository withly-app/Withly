using Withly.Application.Events.Dtos;

namespace Withly.Application.Services;

public interface IEventFetcher
{
    Task<EventDetailsDto?> GetByIdAsync(Guid eventId, CancellationToken ct);
}