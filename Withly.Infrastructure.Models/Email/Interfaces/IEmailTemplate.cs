namespace Withly.Infrastructure.Models.Email.Interfaces;

public interface IEmailTemplate
{
    string To { get; }
    string Subject { get; }
    string TemplateName { get; }
}