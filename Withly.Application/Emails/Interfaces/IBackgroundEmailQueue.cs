namespace Withly.Application.Emails.Interfaces;

public interface IBackgroundEmailQueue
{
    void QueueEmail<T>(T email) where T : IEmailTemplate;
}