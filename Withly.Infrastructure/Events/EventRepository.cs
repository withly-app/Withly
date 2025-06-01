using Microsoft.EntityFrameworkCore;
using Withly.Domain.Entities;
using Withly.Domain.Repositories;
using Withly.Persistence;

namespace Withly.Infrastructure.Events;

public class EventRepository(AppDbContext dbContext) : IEventRepository
{
    public async Task AddAsync(Event e, CancellationToken ct)
    {
        await dbContext.Events.AddAsync(e, ct);
    }
    
    public async Task<Event?> GetByIdWithInviteesAsync(Guid id, CancellationToken ct)
    {
        return await dbContext.Events
            .Include(e => e.Invitees)
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }

}