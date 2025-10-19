using Microsoft.EntityFrameworkCore;
using Withly.Application.Common.Interfaces;
using Withly.Application.Events.Dtos;
using Withly.Persistence;
using Withly.Persistence.Entities;

namespace Withly.Application.Events;

internal class EventCreator(
    AppDbContext dbContext,
    ICurrentUserService currentUser,
    IEventMailer eventMailer) : IEventCreator
{
    public async Task<Guid> CreateEventAsync(CreateEventDto request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedAccessException();

        var organizer = await dbContext.UserProfiles
            .Include(u => u.User)
            .FirstAsync(u => u.Id == userId, ct);
        
        var invitees = await dbContext.Invitees
            .Where(i => request.InviteeEmails.Contains(i.Email))
            .ToListAsync(ct);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
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
        
        var privateEventHasInvitees = !@event.IsPublic && request.InviteeEmails is { Count: > 0 };

        if (privateEventHasInvitees)
        {
            foreach (var email in request.InviteeEmails)
            {
                if (invitees.FirstOrDefault(i => i.Email.Equals(email, StringComparison.OrdinalIgnoreCase)) is { } existingInvitee)
                {
                    @event.AddInvitee(existingInvitee);
                    continue;
                }
                
                @event.AddInvitee(new Invitee(email));
            }
        }

        
        await dbContext.Events.AddAsync(@event, ct);

        await dbContext.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        

        if (privateEventHasInvitees)
        {
            eventMailer.SendEventInvite(@event, organizer);
        }

        return @event.Id;
    }
}