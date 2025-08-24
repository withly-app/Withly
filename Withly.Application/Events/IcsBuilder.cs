using System.Text;
using Withly.Domain.Entities;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;

namespace Withly.Application.Events;

public static class IcsBuilder
{
    public static byte[] BuildEventInvite(Event @event)
    {
        var calendar = new Calendar();

        var e = new CalendarEvent
        {
            Summary = @event.Title,
            Description = @event.Description,
            DtStart = new CalDateTime(@event.StartUtc, "UTC"),
            DtEnd = new CalDateTime(@event.EndUtc, "UTC"),
            Uid = @event.Id.ToString(),
            Created = new CalDateTime(@event.CreatedUtc, "UTC"),
            Transparency = TransparencyType.Opaque
        };

        if (!string.IsNullOrWhiteSpace(@event.RecurringRule))
            e.RecurrenceRules.Add(new RecurrencePattern(@event.RecurringRule));

        calendar.Events.Add(e);

        var serializer = new CalendarSerializer();
        var serialized = serializer.SerializeToString(calendar);

        return Encoding.UTF8.GetBytes(serialized);
    }
}
