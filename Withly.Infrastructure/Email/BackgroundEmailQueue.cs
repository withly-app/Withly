using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Withly.Persistence.Entities.Interfaces;

namespace Withly.Infrastructure.Email;

public class BackgroundEmailQueue(ILogger<BackgroundEmailQueue> logger) : IBackgroundEmailQueue
{
    private readonly Channel<IEnumerable<IEmailTemplate>> _queue = Channel.CreateUnbounded<IEnumerable<IEmailTemplate>>();
    public ChannelReader<IEnumerable<IEmailTemplate>> Reader => _queue.Reader;
    public void QueueEmail<T>(T email, Guid? userId) where T : class, IEmailTemplate
    {
        email.UserId = userId;
        if (!_queue.Writer.TryWrite([email]))
        {
            logger.LogError("Could not write email to queue");
        }
    }
    
    public void QueueEmail<T>(IEnumerable<T> emails, Guid? userId) where T : class, IEmailTemplate
    {
        if (!_queue.Writer.TryWrite(emails))
        {
            logger.LogError("Could not write email to queue");
        }
    }
}