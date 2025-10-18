using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Withly.Persistence.Entities;

public class Event
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public DateTime CreatedUtc { get; private set; } = DateTime.UtcNow;
    public Guid OrganizerId { get; private set; }
    [MaxLength(100)]
    public string Title { get; private set; } = string.Empty;
    [MaxLength(500)]
    public string? Description { get; private set; }
    public DateTime StartUtc { get; private set; }
    public DateTime EndUtc { get; private set; }
    public bool IsRecurring { get; private set; }
    [MaxLength(200)]
    public string? RecurringRule { get; private set; } // RFC 5545 format (null = one-time)
    public bool IsPublic { get; private set; }
    [MaxLength(50)]
    public string? PublicJoinCode { get; private set; } // For public URLs
    public List<Invitee> Invitees { get; private set; } = [];
    public List<Rsvp> Rsvps { get; private set; } = [];

    [UsedImplicitly]
    private Event() { } // For EF Core

    public Event(Guid organizerId, string title, string? description, DateTime startUtc, DateTime endUtc, bool isRecurring, string? recurringRule = null, bool isPublic = false)
    {
        OrganizerId = organizerId;
        Title = title;
        Description = description;
        StartUtc = startUtc;
        EndUtc = endUtc;
        IsRecurring = isRecurring;
        RecurringRule = recurringRule;
        IsPublic = isPublic;

        
        if (isPublic)
        {
            PublicJoinCode = GenerateJoinCode(); // e.g., random string or short guid
        }
        
        if (endUtc < startUtc)
            throw new ArgumentException("End time must be after start time");
        
        switch (isRecurring)
        {
            case true when string.IsNullOrWhiteSpace(recurringRule):
                throw new ArgumentException("Recurring events must have a recurrence rule.");
            case false when !string.IsNullOrWhiteSpace(recurringRule):
                throw new ArgumentException("Non-recurring events cannot have a recurrence rule.");
        }
    }

    public void AddInvitee(Invitee invitee)
    {
        if (IsPublic)
            throw new InvalidOperationException("Cannot add invitees to public events.");

        if (HasInvitee(invitee.Email))
            return;

        Invitees.Add(invitee);
    }
    
    public bool HasInvitee(string email) =>
        Invitees.Any(i => i.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase));

    private static string GenerateJoinCode()
    {
        return Guid.NewGuid().ToString("N")[..8]; // Example: short join code
    }
}