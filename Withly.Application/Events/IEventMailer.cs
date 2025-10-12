using Withly.Persistence.Entities;

namespace Withly.Application.Events;

public interface IEventMailer
{
    void SendEventInvite(Event @event);
}