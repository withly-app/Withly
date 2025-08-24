using Withly.Infrastructure.Models.Email.Interfaces;

namespace Withly.Application.Emails.Templates;

public class WelcomeEmail : IEmailTemplate
{
    public string To { get; init; } = null!;
    public string DisplayName { get; init; } = null!;
    public string TemplateName => "Welcome";
    public string Subject => "Welcome to Withly!";
}