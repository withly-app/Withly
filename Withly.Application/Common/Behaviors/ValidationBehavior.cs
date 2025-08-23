using FluentValidation;

namespace Withly.Application.Common.Behaviors;

public class RequestValidator<TRequest>(IEnumerable<IValidator<TRequest>> validators)
    where TRequest : notnull
{
    public async Task Handle(TRequest request, CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return;

        var context = new ValidationContext<TRequest>(request);

        var failures = (await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken))))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count == 0) return;

        throw new ValidationException("Validation failed", failures);
    }
}
