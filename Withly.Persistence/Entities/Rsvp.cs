using System.ComponentModel.DataAnnotations;
using Withly.Domain.Enums;

namespace Withly.Persistence.Entities;

public class Rsvp
{
    public Guid EventId { get; private set; }
    public Event Event { get; private set; } = null!;
    public Guid InviteeId { get; private set; }
    public Invitee Invitee { get; private set; } = null!;
    public RsvpStatus Status { get; private set; }
    public DateTime RespondedAtUtc { get; private set; } = DateTime.UtcNow;
    
    [MaxLength(256)]
    public string Secret { get; private set; } = string.Empty;
}