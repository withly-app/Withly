using Withly.Domain.Entities;

namespace Withly.Application.Events;

public interface IEventMailer
{
    void SendEventInvite(Event @event, CancellationToken ct = default);
}