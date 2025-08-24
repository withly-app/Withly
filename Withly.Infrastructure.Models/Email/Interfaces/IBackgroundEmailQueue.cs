namespace Withly.Infrastructure.Models.Email.Interfaces;

public interface IBackgroundEmailQueue
{
    void QueueEmail<T>(T email) where T : IEmailTemplate;
}