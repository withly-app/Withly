using Withly.Application.Common.Interfaces;
using Withly.Application.Events.Commands;
using Withly.Application.Events.Dtos;
using Withly.Domain.Entities;
using Withly.Domain.Repositories;

namespace Withly.Application.Services;

public class EventService(
    IEventRepository repository,
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork)
{
    public async Task<EventDetailsDto?> GetByIdAsync(Guid eventId, CancellationToken ct)
    {
        var @event = await repository.GetByIdWithInviteesAsync(eventId, ct);
        if (@event is null) return null;

        return new EventDetailsDto(
            @event.Id,
            @event.Title,
            @event.Description,
            @event.StartUtc,
            @event.EndUtc,
            @event.IsRecurring,
            @event.RecurringRule,
            @event.IsPublic,
            @event.PublicJoinCode,
            @event.IsPublic
                ? [] // No invitees for public events
                : @event.Invitees
                    .Select(i => new InviteeDto(i.Email, i.Name, i.RsvpStatus.ToString()))
                    .ToList()
        );
    }
    
    public async Task<Guid> CreateEventAsync(CreateEventObject request, CancellationToken ct)
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

        await repository.AddAsync(@event, ct);
        await unitOfWork.SaveChangesAsync(ct);
        
        return @event.Id;
    }
}