using System.ComponentModel.DataAnnotations;

namespace Withly.Domain.Entities;

public class UserProfile
{
    public Guid Id { get; set; }
    [MaxLength(50)]
    public string DisplayName { get; set; }
    public string AvatarUrl { get; set; }
}