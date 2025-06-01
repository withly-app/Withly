namespace Withly.Application.Events.Dtos;

public record EventDetailsDto(
    Guid Id,
    string Title,
    string Description,
    DateTime StartUtc,
    DateTime EndUtc,
    bool IsRecurring,
    string? RecurringRule,
    bool IsPublic,
    string? PublicJoinCode,
    List<InviteeDto> Invitees
);