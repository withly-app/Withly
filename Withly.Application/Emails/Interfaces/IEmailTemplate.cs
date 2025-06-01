namespace Withly.Application.Emails.Interfaces;

public interface IEmailTemplate
{
    string To { get; }
    string Subject { get; }
    string TemplateName { get; }
}