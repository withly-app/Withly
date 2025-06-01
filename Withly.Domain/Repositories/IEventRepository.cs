using Withly.Domain.Entities;

namespace Withly.Domain.Repositories;

public interface IEventRepository
{
    Task AddAsync(Event @event, CancellationToken ct = default);
    Task<Event?> GetByIdWithInviteesAsync(Guid id, CancellationToken ct = default);
}