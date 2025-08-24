namespace Withly.Infrastructure.Models.Email;

public class EmailAttachment
{
    public Guid Id { get; set; }
    public Guid EmailId { get; set; }
    public EmailMessage Email { get; } = null!;
    
    public required string FileName { get; set; }
    public required string MimeType { get; set; }
    
    public byte[] Content { get; set; } = [];
    
    public DateTime CreatedUtc { get; } = DateTime.UtcNow;
}