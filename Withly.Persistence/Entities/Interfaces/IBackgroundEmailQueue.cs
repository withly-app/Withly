namespace Withly.Persistence.Entities.Interfaces;

public interface IBackgroundEmailQueue
{
    void QueueEmail<T>(T email, Guid? userId) where T : IEmailTemplate;
}