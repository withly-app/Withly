using Withly.Infrastructure.Models.Email.Interfaces;
namespace Withly.Infrastructure.Models.Email;

public class EmailMessage
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public List<string> Recipients { get; init; }
    public string Subject { get; init; }
    public string Body { get; init; }

    public List<EmailAttachment> Attachments { get; init; } = [];
    
    public DateTime CreatedUtc { get; init; } = DateTime.UtcNow;
    public int RetryCount { get; set; }
    public EmailStatus Status { get; set; } = EmailStatus.Queued;
    public DateTime SentUtc { get; set; }
    public DateTime NextAttemptUtc { get; set; } = DateTime.UtcNow;

    public EmailMessage(){}
    public EmailMessage(IEmailTemplate template, string body)
    {
        Id = Guid.NewGuid();
        UserId = template.UserId;
        Recipients = [template.To];
        Subject = template.Subject;
        Body = body;
    }
}

public enum EmailStatus
{
    None,
    Queued,
    InProgress,
    Completed,
    Failed
}