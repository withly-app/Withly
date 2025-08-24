using Withly.Domain.Entities;

namespace Withly.Infrastructure.Models.Email.Interfaces;

public interface IAttachmentFactory
{
    EmailAttachment CreateCalendarAttachment(Event @event, byte[] icsBytes);
}