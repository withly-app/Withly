namespace Withly.Persistence.Entities.Interfaces;

public interface IEmailService
{
    Task SendAsync(EmailMessage emailModel, CancellationToken ct = default);
}