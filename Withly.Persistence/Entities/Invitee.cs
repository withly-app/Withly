using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Withly.Persistence.Entities;

public class Invitee
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    
    public List<Event> Events { get; private set; } = [];
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; private set; } = string.Empty;
    [MaxLength(100)]
    public string? Name { get; private set; }

    public List<Rsvp> Rsvps { get; private set; } = [];

    public Guid? UserId { get; private set; } // If they are a Withly user

    [UsedImplicitly]
    private Invitee() { } // EF

    public Invitee(string email, string? name = null)
    {
        Email = email.Trim().ToLowerInvariant();
        Name = name;
    }
}