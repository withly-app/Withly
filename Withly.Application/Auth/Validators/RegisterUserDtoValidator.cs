using FluentValidation;
using Withly.Application.Auth.Dtos;

namespace Withly.Application.Auth.Validators;

public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserDtoValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is invalid.");
        
        RuleFor(command => command.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.");
        
        RuleFor(command => command.DisplayName)
            .NotEmpty().WithMessage("Display name is required.")
            .MinimumLength(2).WithMessage("Display name must be at least 2 characters.")
            .MaximumLength(50).WithMessage("Display name must be at most 50 characters.")
            .Matches(@"^[\p{L}0-9 .,'\-_]+$").WithMessage("Display name contains invalid characters.");
    }
}