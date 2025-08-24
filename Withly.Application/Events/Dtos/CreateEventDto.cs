namespace Withly.Application.Events.Commands;

public record CreateEventDto(
    string Title,
    string Description,
    DateTime StartUtc,
    DateTime EndUtc,
    bool IsRecurring,
    string? RecurringRule,
    bool IsPublic,
    List<string> InviteeEmails);