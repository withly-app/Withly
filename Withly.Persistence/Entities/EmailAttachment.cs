using System.ComponentModel.DataAnnotations;

namespace Withly.Persistence.Entities;

public class EmailAttachment
{
    public Guid Id { get; set; }
    public List<EmailMessage> Emails { get; } = null!;
    
    [MaxLength(255)]
    public required string FileName { get; set; }
    [MaxLength(100)]
    public required string MimeType { get; set; }
    
    public byte[] Content { get; set; } = [];
    
    public DateTime CreatedUtc { get; } = DateTime.UtcNow;
}