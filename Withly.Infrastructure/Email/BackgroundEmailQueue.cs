using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Withly.Infrastructure.Models.Email.Interfaces;

namespace Withly.Infrastructure.Email;

public class BackgroundEmailQueue(ILogger<BackgroundEmailQueue> logger) : IBackgroundEmailQueue
{
    private readonly Channel<IEmailTemplate> _queue = Channel.CreateUnbounded<IEmailTemplate>();
    public ChannelReader<IEmailTemplate> Reader => _queue.Reader;
    public void QueueEmail<T>(T email, Guid? userId) where T : IEmailTemplate
    {
        email.UserId = userId;
        if (!_queue.Writer.TryWrite(email))
        {
            logger.LogError("Could not write email to queue");
        }
    }
}