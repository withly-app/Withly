namespace Withly.Application.Events.Dtos;

public class CreateEventDto
{
    /// <summary>
    /// Title of the event.
    /// </summary>
    /// <example>Awakenings 2026</example>
    public required string Title { get; init; }

    /// <summary>
    /// Short description of what the event is about.
    /// </summary>
    /// <example>A techno event at Hilvarenbeek</example>
    public string? Description { get; init; }

    /// <summary>
    /// Start date and time of the event in UTC.
    /// </summary>
    /// <example>2026-06-14T18:00:00Z</example>
    public DateTime StartUtc { get; init; }

    /// <summary>
    /// End date and time of the event in UTC.
    /// </summary>
    /// <example>2026-06-15T02:00:00Z</example>
    public DateTime EndUtc { get; init; }

    /// <summary>
    /// Indicates whether the event repeats.
    /// </summary>
    /// <example>false</example>
    public bool IsRecurring { get; init; }

    /// <summary>
    /// Recurrence rule in <c>RFC 5545</c> format.
    /// Required if <see cref="IsRecurring"/> is <c>true</c>.
    /// </summary>
    /// <example>FREQ=WEEKLY;BYDAY=FR,SA</example>
    public string? RecurringRule { get; init; }

    /// <summary>
    /// Whether the event is public. Public events can be joined by a link.
    /// </summary>
    /// <example>false</example>
    public bool IsPublic { get; init; }

    /// <summary>
    /// List of invitee email addresses. Only valid for private events.
    /// </summary>
    /// <example>
    /// [ "alice@example.com", "bob@example.org" ]
    /// </example>
    public List<string> InviteeEmails { get; init; } = [];
}