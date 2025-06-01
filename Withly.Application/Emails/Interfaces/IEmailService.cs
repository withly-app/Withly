namespace Withly.Application.Emails.Interfaces;

public interface IEmailService
{
    Task SendAsync<T>(T emailModel, CancellationToken ct = default)
        where T : IEmailTemplate;
}