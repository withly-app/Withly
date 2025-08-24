using Withly.Infrastructure.Models.Email;
using Withly.Infrastructure.Models.Email.Interfaces;

namespace Withly.Application.Emails.Templates;

public class PasswordResetEmail : IEmailTemplate
{
    public string To { get; init; } = null!;
    public Guid? UserId { get; set; }
    public string Username { get; init; } = null!;
    public string ResetLink { get; init; } = null!;
    public List<EmailAttachment> Attachments { get; }
    public string Subject => "Withly Password Reset";
    public string TemplateName => "PasswordReset";
}