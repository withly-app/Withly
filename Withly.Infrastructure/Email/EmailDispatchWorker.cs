using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Withly.Persistence;
using Withly.Persistence.Entities;
using Withly.Persistence.Entities.Interfaces;

namespace Withly.Infrastructure.Email;

public class EmailDispatchWorker(
    ILogger<EmailDispatchWorker> logger,
    IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var smtp = scope.ServiceProvider.GetRequiredService<IEmailService>();

            await using var transaction = await dbContext.Database.BeginTransactionAsync(stoppingToken);
            try
            {
                
                var pending = dbContext.EmailMessages
                    .Where(m => m.Status == EmailStatus.Queued && m.NextAttemptUtc <= DateTime.UtcNow)
                    .OrderBy(m => m.CreatedUtc)
                    .Take(10);

                foreach (var msg in pending)
                {
                    try
                    {
                        msg.Status = EmailStatus.InProgress;

                        await smtp.SendAsync(msg, stoppingToken);
                        msg.Status = EmailStatus.Completed;

                        logger.LogInformation("Sent email {Id} to {Recipients}", msg.Id, msg.Recipients);
                    }
                    catch (Exception ex)
                    {
                        var retryCount = msg.RetryCount;
                        logger.LogError(ex, "Failed to send email {Id}, retry count: {RetryCount}", msg.Id, retryCount);

                        if (msg.RetryCount < 4)
                        {
                            msg.Status = EmailStatus.Queued;
                            msg.RetryCount++;
                            msg.NextAttemptUtc = DateTime.UtcNow.AddSeconds(5 * msg.RetryCount);
                            continue;
                        }

                        msg.Status = EmailStatus.Failed;
                    }
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(stoppingToken);
                logger.LogError(ex, "Error in EmailDispatchWorker loop");
            }
            finally
            {
                await dbContext.SaveChangesAsync(stoppingToken);
                await transaction.CommitAsync(stoppingToken);
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}