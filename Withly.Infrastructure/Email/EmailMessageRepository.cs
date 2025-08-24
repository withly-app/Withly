using Microsoft.EntityFrameworkCore;
using Withly.Infrastructure.Models.Email;
using Withly.Persistence;

namespace Withly.Infrastructure.Email;

public class EmailMessageRepository(AppDbContext db)
{
    public async Task EnqueueAsync(EmailMessage msg, CancellationToken ct)
    {
        db.EmailMessages.Add(msg);
        await db.SaveChangesAsync(ct);
    }

    public async Task<List<EmailMessage>> GetPendingAsync(int batchSize, CancellationToken ct)
        => await db.EmailMessages
            .Where(m => m.Status == EmailStatus.Queued && m.NextAttemptUtc <= DateTime.UtcNow)
            .OrderBy(m => m.CreatedUtc)
            .Take(batchSize)
            .ToListAsync(ct);

    public async Task<List<EmailMessage>> GetByUserIdAsync(Guid userId, CancellationToken ct)
        => await db.EmailMessages
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.CreatedUtc)
            .ToListAsync(ct);

    public async Task MarkAsInProgressAsync(Guid id, CancellationToken ct)
    {
        var msg = await db.EmailMessages.FindAsync([id], ct);
        if (msg is not null)
        {
            msg.Status = EmailStatus.InProgress;
            await db.SaveChangesAsync(ct);
        }
    }
    
    public async Task MarkAsFailedAsync(Guid id, CancellationToken ct)
    {
        var msg = await db.EmailMessages.FindAsync([id], ct);
        if (msg is not null)
        {
            msg.Status = EmailStatus.Failed;
            await db.SaveChangesAsync(ct);
        }
    }
    
    public async Task MarkAsSentAsync(Guid id, CancellationToken ct)
    {
        var msg = await db.EmailMessages.FindAsync([id], ct);
        if (msg is not null)
        {
            msg.Status = EmailStatus.Completed;
            msg.SentUtc = DateTime.UtcNow;
            await db.SaveChangesAsync(ct);
        }
    }

    public async Task IncrementRetryAnRequeueAsync(Guid id, CancellationToken ct)
    {
        var msg = await db.EmailMessages.FindAsync([id], ct);
        if (msg is not null)
        {
            msg.Status = EmailStatus.Queued;
            msg.RetryCount++;
            msg.NextAttemptUtc = DateTime.UtcNow.AddSeconds(5 * msg.RetryCount);
            await db.SaveChangesAsync(ct);
        }
    }
}