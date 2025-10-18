using Microsoft.AspNetCore.Identity;
using Withly.Persistence.Entities;

namespace Withly.Persistence;

public class ApplicationUser : IdentityUser<Guid>
{
#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
    public override string Email { get; set; } = null!;
    public override string NormalizedEmail { get; set; } = null!;
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).

    public UserProfile Profile { get; set; } = null!;
    public List<EmailMessage> Emails { get; } = [];
}