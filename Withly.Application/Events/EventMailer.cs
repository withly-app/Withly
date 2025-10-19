using Withly.Application.Emails.Templates;
using Withly.Persistence.Entities;
using Withly.Persistence.Entities.Interfaces;

namespace Withly.Application.Events;

internal class EventMailer(
    IBackgroundEmailQueue mailQueue,
    IAttachmentFactory attachmentFactory) : IEventMailer
{
    public void SendEventInvite(Event @event, UserProfile organizer)
    {
        var icsBytes = IcsBuilder.BuildEventInvite(@event);
        var attachment = attachmentFactory.CreateCalendarAttachment(@event, icsBytes);

        List<EventInviteEmail> emails = [];
        emails.AddRange(@event.Invitees.Select(invitee => BuildInvite(@event, invitee, attachment)));

        emails.Add(BuildInvite(@event, organizer, attachment));
        
        mailQueue.QueueEmail(emails, organizer.Id);
    }

    private static EventInviteEmail BuildInvite(Event @event, Invitee invitee, EmailAttachment attachment)
    {
        return new EventInviteEmail
        {
            DisplayName = invitee.Name ?? "",
            EventId = @event.Id,
            EventTitle = @event.Title,
            StartUtc = @event.StartUtc,
            To = invitee.Email,
            Attachments =
            [
                attachment
            ],
            InviteeId = invitee.Id,
            RsvpSecret = invitee.Rsvps.First(e => e.EventId == @event.Id).Secret
        };
    }

    private static EventInviteEmail BuildInvite(Event @event, UserProfile organizer, EmailAttachment attachment)
    {
        // Organizer also gets an invitation; this should, however, be changed to not include RSVP links
        // TODO: Implement organizer invite without RSVP links
        return new EventInviteEmail
        {
            DisplayName = organizer.DisplayName,
            EventId = @event.Id,
            EventTitle = @event.Title,
            StartUtc = @event.StartUtc,
            To = organizer.User.Email,
            Attachments =
            [
                attachment
            ],
            InviteeId = Guid.Empty,
            RsvpSecret = ""
        };
    }
}