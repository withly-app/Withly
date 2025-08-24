using Withly.Application.Events.Dtos;

namespace Withly.Application.Events;

public interface IEventCreator
{
    Task<Guid> CreateEventAsync(CreateEventDto request, CancellationToken ct);
}