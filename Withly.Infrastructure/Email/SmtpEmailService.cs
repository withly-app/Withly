using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Withly.Infrastructure.Models.Email;
using Withly.Infrastructure.Models.Email.Interfaces;

namespace Withly.Infrastructure.Email;

public class SmtpEmailService(
    IEmailTemplateRenderer renderer,
    IOptions<SmtpSettings> smtpSettingsOptions,
    ILogger<SmtpEmailService> logger) : IEmailService
{
    private readonly SmtpSettings _smtpSettings = smtpSettingsOptions.Value;

    public async Task SendAsync(EmailMessage emailModel, CancellationToken ct = default)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse("noreply@withly.app"));
        foreach (var recipient in emailModel.Recipients)
        {
            if (MailboxAddress.TryParse(recipient, out var mailboxAddress))
            {
                message.To.Add(mailboxAddress);
            }
        }
        message.Subject = emailModel.Subject;
        message.Body = new TextPart("html") { Text = emailModel.Body };

        using var client = new SmtpClient();
        client.CheckCertificateRevocation = false;
        await client.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, SecureSocketOptions.StartTls, ct);
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