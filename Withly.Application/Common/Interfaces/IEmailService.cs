namespace Withly.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendAsync<T>(T emailModel, CancellationToken ct = default)
        where T : IEmailTemplate;
}