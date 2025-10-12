using System.Text;
using Withly.Persistence.Entities;
using Withly.Persistence.Entities.Interfaces;

namespace Withly.Infrastructure.Email;
public class AttachmentFactory : IAttachmentFactory
{
    public EmailAttachment CreateCalendarAttachment(Event @event, byte[] icsBytes)
    {
        var safeTitle = NormalizeFileName(@event.Title);

        return new EmailAttachment
        {
            Content = icsBytes,
            FileName = $"{safeTitle}.ics",
            MimeType = "text/calendar"
        };
    }

    /// <summary>
    /// Normalizes an input string into a safe file name.
    /// </summary>
    /// <param name="input">
    /// The raw input string to normalize. May contain spaces, invalid characters,
    /// or mixed casing.
    /// </param>
    /// <param name="maxLength">
    /// The maximum length of the resulting file name. Defaults to 100 characters.
    /// </param>
    /// <returns>
    /// A normalized file name string where:
    /// <list type="bullet">
    ///   <item><description>Spaces are replaced with underscores (<c>_</c>).</description></item>
    ///   <item><description>Invalid file name characters are removed.</description></item>
    ///   <item><description>All characters are converted to lowercase using invariant culture.</description></item>
    ///   <item><description>The string is truncated to the specified maximum length.</description></item>
    ///   <item><description>If the result is empty, the fallback value <c>"unnamed"</c> is returned.</description></item>
    /// </list>
    /// </returns>
    private static string NormalizeFileName(string input, int maxLength = 100)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "unnamed";

        var invalidChars = Path.GetInvalidFileNameChars().ToHashSet();
        var normalized = new StringBuilder(input.Length);

        foreach (var ch in input)
        {
            if (ch == ' ')
            {
                normalized.Append('_');
                continue;
            }

            if (invalidChars.Contains(ch))
                continue;

            normalized.Append(char.ToLowerInvariant(ch));

            if (normalized.Length >= maxLength)
                break;
        }

        return normalized.Length > 0
            ? normalized.ToString()
            : "unnamed";
    }
}
