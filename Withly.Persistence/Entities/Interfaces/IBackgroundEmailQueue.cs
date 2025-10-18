namespace Withly.Persistence.Entities.Interfaces;

public interface IBackgroundEmailQueue
{
    void QueueEmail<T>(T email, Guid? userId) where T : class, IEmailTemplate;
    void QueueEmail<T>(IEnumerable<T> emails, Guid? userId) where T : class, IEmailTemplate;
}