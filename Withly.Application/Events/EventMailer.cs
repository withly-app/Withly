using Withly.Application.Emails.Templates;
using Withly.Domain.Entities;
using Withly.Infrastructure.Models.Email.Interfaces;

namespace Withly.Application.Events;

internal class EventMailer(IBackgroundEmailQueue mailQueue,
    IAttachmentFactory attachmentFactory) : IEventMailer
{
    public void SendEventInvite(Event @event)
    {
        var icsBytes = IcsBuilder.BuildEventInvite(@event);
        var attachment = attachmentFactory.CreateCalendarAttachment(@event, icsBytes);

        foreach (var invitee in @event.Invitees)
        {
            mailQueue.QueueEmail(new EventInviteEmail
            {
                DisplayName = invitee.Name ?? "",
                EventId = @event.Id,
                EventTitle = @event.Title,
                StartUtc = @event.StartUtc,
                To = invitee.Email,
                Attachments = [attachment]
            }, invitee.UserId);
        }
    }
}