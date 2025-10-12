using System.ComponentModel.DataAnnotations;

namespace Withly.Persistence.Entities;

public class UserProfile
{
    [Key]
    public Guid Id { get; init; }
    public ApplicationUser User { get; init; } = null!;
    [MaxLength(50)]
    public required string DisplayName { get; set; }
    
    [MaxLength(500)]
    public required string AvatarUrl { get; set; }
    
}