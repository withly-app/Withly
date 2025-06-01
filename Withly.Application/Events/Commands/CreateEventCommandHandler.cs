using MediatR;
using Withly.Application.Common.Interfaces;
using Withly.Domain.Entities;
using Withly.Domain.Repositories;

namespace Withly.Application.Events.Commands;

public class CreateEventCommandHandler(
    IEventRepository repository,
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateEventCommand, Guid>
{
    public async Task<Guid> Handle(CreateEventCommand request, CancellationToken ct)
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