using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Withly.Persistence;
using Withly.Persistence.Entities;
using Withly.Persistence.Entities.Interfaces;

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
                var razorRenderer = scope.ServiceProvider.GetRequiredService<IEmailTemplateRenderer>();
                var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                string html;
                try
                {
                    html = await razorRenderer.RenderAsync(email);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error rendering email");
                    continue;
                }
                
                logger.LogInformation("Rendered {EmailType} email", email.TemplateName);
                
                var emailMessage = new EmailMessage(email, html);
                await appDbContext.EmailMessages.AddAsync(emailMessage, stoppingToken);
                
                logger.LogInformation("Enqueued {EmailType} email", email.TemplateName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error sending email");
            }
        }
    }
}