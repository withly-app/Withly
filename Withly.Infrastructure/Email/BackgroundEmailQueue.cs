using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Withly.Infrastructure.Models.Email.Interfaces;

namespace Withly.Infrastructure.Email;

public class BackgroundEmailQueue(ILogger<BackgroundEmailQueue> logger) : IBackgroundEmailQueue
{
    private readonly Channel<IEmailTemplate> _queue = Channel.CreateUnbounded<IEmailTemplate>();
    public ChannelReader<IEmailTemplate> Reader => _queue.Reader;
    public void QueueEmail<T>(T email) where T : IEmailTemplate
    {
        if (!_queue.Writer.TryWrite(email))
        {
            logger.LogError("Could not write email to queue");
        }
    }
}