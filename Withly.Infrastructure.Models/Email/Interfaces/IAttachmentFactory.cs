using Withly.Domain.Entities;
using Withly.Infrastructure.Models.Email;

namespace Withly.Infrastructure.Email;

public interface IAttachmentFactory
{
    EmailAttachment CreateCalendarAttachment(Event @event, byte[] icsBytes);
}