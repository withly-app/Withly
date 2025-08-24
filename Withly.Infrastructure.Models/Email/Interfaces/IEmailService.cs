namespace Withly.Infrastructure.Models.Email.Interfaces;

public interface IEmailService
{
    Task SendAsync(EmailMessage emailModel, CancellationToken ct = default);
}