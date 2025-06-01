using Microsoft.EntityFrameworkCore;
using Withly.Domain.Entities;
using Withly.Domain.Repositories;
using Withly.Persistence;

namespace Withly.Infrastructure.UserProfiles;

public class UserProfileRepository(AppDbContext dbContext) : IUserProfileRepository
{
    public async Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.UserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task AddAsync(UserProfile profile, CancellationToken ct = default)
    {
        await dbContext.UserProfiles.AddAsync(profile, ct);
    }

    public Task UpdateAsync(UserProfile profile, CancellationToken ct = default)
    {
        dbContext.UserProfiles.Update(profile);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.UserProfiles.AnyAsync(x => x.Id == id, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var userProfile = await dbContext.UserProfiles.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (userProfile != null)
            dbContext.UserProfiles.Remove(userProfile);
    }
}