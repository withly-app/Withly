
using Withly.Persistence.Entities;
using Withly.Persistence.Entities.Interfaces;

namespace Withly.Application.Emails.Templates;

public class EventInviteEmail : IEmailTemplate
{
    public required string To { get; init; }
    public Guid? UserId { get; set; }
    public List<EmailAttachment> Attachments { get; set; } = [];
    public string Subject => $"Are you coming along to {EventTitle}";
    public required string DisplayName { get; init; }
    public required string EventTitle { get; init; }
    public required DateTime StartUtc { get; init; }
    public required Guid EventId { get; init; }
    public string RsvpBaseUrl => "https://withly.app/rsvp";
    public string TemplateName => "EventInvite";
}