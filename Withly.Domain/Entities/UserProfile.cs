using System.ComponentModel.DataAnnotations;

namespace Withly.Domain.Entities;

public class UserProfile
{
    public Guid Id { get; init; }
    [MaxLength(50)]
    public required string DisplayName { get; set; }
    public required string AvatarUrl { get; set; }
}