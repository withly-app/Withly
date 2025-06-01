using Withly.Application.Common.Interfaces;
using Withly.Persistence;

namespace Withly.Infrastructure.Common.Services;

public class UnitOfWork(AppDbContext dbContext) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken ct = default) 
        => await dbContext.SaveChangesAsync(ct);
}