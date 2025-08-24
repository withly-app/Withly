using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Withly.Infrastructure.Models.Email.Interfaces;

namespace Withly.Infrastructure.Email;

public class EmailDispatchWorker(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<EmailDispatchWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = serviceScopeFactory.CreateAsyncScope();
                var repo = scope.ServiceProvider.GetRequiredService<EmailMessageRepository>();
                var smtp = scope.ServiceProvider.GetRequiredService<IEmailService>();

                var pending = await repo.GetPendingAsync(10, stoppingToken);

                foreach (var msg in pending)
                {
                    try
                    {
                        await repo.MarkAsInProgressAsync(msg.Id, stoppingToken);

                        await smtp.SendAsync(msg, stoppingToken);
                        await repo.MarkAsSentAsync(msg.Id, stoppingToken);

                        logger.LogInformation("Sent email {Id} to {Recipients}", msg.Id, msg.Recipients);
                    }
                    catch (Exception ex)
                    {
                        var retryCount = msg.RetryCount;
                        logger.LogError(ex, "Failed to send email {Id}, retry count: {RetryCount}", msg.Id, retryCount);
                        if (msg.RetryCount < 4)
                        {
                            await repo.IncrementRetryAnRequeueAsync(msg.Id, stoppingToken);
                            continue;
                        }
                        await repo.MarkAsFailedAsync(msg.Id, stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in EmailDispatchWorker loop");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}