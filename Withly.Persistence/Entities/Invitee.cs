using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Withly.Domain.Enums;

namespace Withly.Persistence.Entities;

public class Invitee
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    
    public Guid EventId { get; private set; }
    public Event Event { get; private set; } = null!;
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; private set; } = string.Empty;
    [MaxLength(100)]
    public string? Name { get; private set; }

    public RsvpStatus RsvpStatus { get; private set; } = RsvpStatus.NoResponse;

    public DateTime? RsvpAtUtc { get; private set; }

    public Guid? UserId { get; private set; } // If they are a Withly user

    [UsedImplicitly]
    private Invitee() { } // EF

    public Invitee(Guid eventId, string email, string? name = null)
    {
        EventId = eventId;
        Email = email.Trim().ToLowerInvariant();
        Name = name;
    }

    public void Respond(RsvpStatus status)
    {
        RsvpStatus = status;
        RsvpAtUtc = DateTime.UtcNow;
    }

    public void LinkToUser(Guid userId)
    {
        UserId = userId;
    }
}