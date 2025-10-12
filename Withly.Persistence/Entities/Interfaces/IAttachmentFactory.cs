
namespace Withly.Persistence.Entities.Interfaces;

public interface IAttachmentFactory
{
    EmailAttachment CreateCalendarAttachment(Event @event, byte[] icsBytes);
}