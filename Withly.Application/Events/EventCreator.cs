using Withly.Application.Common.Interfaces;
using Withly.Application.Events.Dtos;
using Withly.Persistence;
using Withly.Persistence.Entities;

namespace Withly.Application.Events;

internal class EventCreator(AppDbContext dbContext,
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork,
    IEventMailer eventMailer) : IEventCreator
{
    public async Task<Guid> CreateEventAsync(CreateEventDto request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedAccessException();

        var @event = new Event(
            organizerId: userId,
            title: request.Title,
            description: request.Description,
            startUtc: request.StartUtc,
            endUtc: request.EndUtc,
            isRecurring: request.IsRecurring,
            recurringRule: request.RecurringRule,
            isPublic: request.IsPublic
        );

        if (!@event.IsPublic && request.InviteeEmails is { Count: > 0 })
        {
            foreach (var email in request.InviteeEmails)
                @event.AddInvitee(email);
        }

        await dbContext.Events.AddAsync(@event, ct);
        await unitOfWork.SaveChangesAsync(ct);

        if (!@event.IsPublic && request.InviteeEmails is { Count: > 0 })
        {
            eventMailer.SendEventInvite(@event);
        }
        
        return @event.Id;
    }
}