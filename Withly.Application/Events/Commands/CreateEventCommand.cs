using MediatR;

namespace Withly.Application.Events.Commands;

public record CreateEventCommand(
    string Title,
    string Description,
    DateTime StartUtc,
    DateTime EndUtc,
    bool IsRecurring,
    string? RecurringRule,
    bool IsPublic,
    List<string> InviteeEmails) : IRequest<Guid>;