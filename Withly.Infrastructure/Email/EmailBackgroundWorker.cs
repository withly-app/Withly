using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Withly.Application.Emails.Interfaces;

namespace Withly.Infrastructure.Email;

public class EmailBackgroundWorker(
    IBackgroundEmailQueue queue,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<EmailBackgroundWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var reader = (queue as BackgroundEmailQueue)?.Reader;

        if (reader is null)
        {
            logger.LogError("Email queue reader not available");
            return;
        }

        await foreach (var email in reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                await using var scope = serviceScopeFactory.CreateAsyncScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                
                await emailService.SendAsync(email, stoppingToken);
                logger.LogInformation("Sent {EmailType} email", email.TemplateName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error sending email");
            }
        }
    }
}