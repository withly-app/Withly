using System.ComponentModel.DataAnnotations;
using Withly.Persistence.Entities.Interfaces;

namespace Withly.Persistence.Entities;

public class EmailMessage
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public ApplicationUser? User{ get; set; } = null!;
    public List<string> Recipients { get; init; } = [];
    [MaxLength(255)]
    public string Subject { get; init; } = string.Empty;
    [MaxLength(4000)]
    public string Body { get; init; } = string.Empty;

    public List<EmailAttachment> Attachments { get; init; } = [];

    public DateTime CreatedUtc { get; init; } = DateTime.UtcNow;
    public int RetryCount { get; set; }
    public EmailStatus Status { get; set; } = EmailStatus.Queued;
    public DateTime SentUtc { get; set; }
    public DateTime NextAttemptUtc { get; set; } = DateTime.UtcNow;

    public EmailMessage()
    {
    }

    public EmailMessage(IEmailTemplate template, string body)
    {
        Id = Guid.NewGuid();
        UserId = template.UserId;
        Recipients = [template.To];
        Subject = template.Subject;
        Body = body;
        Attachments = template.Attachments;
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