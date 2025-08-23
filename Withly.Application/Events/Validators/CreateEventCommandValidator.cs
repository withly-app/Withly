using FluentValidation;
using JetBrains.Annotations;
using Withly.Application.Events.Commands;

namespace Withly.Application.Events.Validators;

[UsedImplicitly]
public class CreateEventCommandValidator : AbstractValidator<CreateEventObject>
{
    public CreateEventCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(1000);

        RuleFor(x => x.EndUtc)
            .GreaterThan(x => x.StartUtc)
            .WithMessage("End time must be after start time.");

        RuleFor(x => x.RecurringRule)
            .NotEmpty().When(x => x.IsRecurring)
            .WithMessage("Recurring events must have a recurrence rule.");

        RuleFor(x => x.RecurringRule)
            .Must(string.IsNullOrWhiteSpace).When(x => !x.IsRecurring)
            .WithMessage("Non-recurring events should not have a recurrence rule.");

        RuleForEach(x => x.InviteeEmails)
            .EmailAddress().WithMessage("One or more invitee emails are invalid.");
    }
}