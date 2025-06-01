using Withly.Application.Common.Interfaces;
using Withly.Application.Emails.Interfaces;

namespace Withly.Application.Emails.Templates;

public class PasswordResetEmail : IEmailTemplate
{
    public string To { get; init; } = null!;
    public string Username { get; init; } = null!;
    public string ResetLink { get; init; } = null!;
    public string Subject => "Withly Password Reset";
    public string TemplateName => "PasswordReset";
}