namespace Withly.Application.Events.Commands;

public record CreateEventObject(
    string Title,
    string Description,
    DateTime StartUtc,
    DateTime EndUtc,
    bool IsRecurring,
    string? RecurringRule,
    bool IsPublic,
    List<string> InviteeEmails);