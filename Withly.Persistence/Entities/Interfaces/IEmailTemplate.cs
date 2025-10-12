namespace Withly.Persistence.Entities.Interfaces;

public interface IEmailTemplate
{
    string To { get; }
    Guid? UserId { get; set; }
    List<EmailAttachment> Attachments { get; }
    string Subject { get; }
    string TemplateName { get; }
}