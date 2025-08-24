using Withly.Infrastructure.Models.Email.Interfaces;

namespace Withly.Application.Emails.Templates;

public class EventInviteEmail : IEmailTemplate
{
    public required string To { get; init; }
    public string Subject => $"Are you coming along to {EventTitle}";
    public required string DisplayName { get; init; }
    public required string EventTitle { get; init; }
    public required DateTime StartUtc { get; init; }
    public required Guid EventId { get; init; }
    public static string RsvpBaseUrl => "https://withly.app/rsvp";
    public string TemplateName => "EventInvite";
}