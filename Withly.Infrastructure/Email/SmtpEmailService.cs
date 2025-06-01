using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Withly.Application.Common.Interfaces;
using Withly.Application.Emails.Interfaces;
using Withly.Infrastructure.Models.Email;

namespace Withly.Infrastructure.Email;

public class SmtpEmailService(
    IEmailTemplateRenderer renderer,
    IOptions<SmtpSettings> smtpSettingsOptions,
    ILogger<SmtpEmailService> logger) : IEmailService
{
    private readonly SmtpSettings _smtpSettings = smtpSettingsOptions.Value;

    public async Task SendAsync<T>(T emailModel, CancellationToken ct = default) where T : IEmailTemplate
    {
        var html = await renderer.RenderAsync(emailModel);
        if (html is null)
            return;

        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse("noreply@withly.app"));
        message.To.Add(MailboxAddress.Parse(emailModel.To));
        message.Subject = emailModel.Subject;
        message.Body = new TextPart("html") { Text = html };

        using var client = new SmtpClient();
        await client.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, SecureSocketOptions.StartTlsWhenAvailable, ct);
        await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password, ct);

        try
        {
            await client.SendAsync(message, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occured while sending an email");
        }
        finally
        {
            await client.DisconnectAsync(true, ct);
        }
    }
}