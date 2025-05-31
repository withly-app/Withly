using MediatR;
using Microsoft.AspNetCore.Identity;
using Withly.Application.Common;
using Withly.Application.Common.Interfaces;
using Withly.Application.Emails.Templates;
using Withly.Persistence;

namespace Withly.Application.Auth.Commands;

public class RequestPasswordResetCommandHandler(UserManager<ApplicationUser> userManager, IEmailService emailService) : IRequestHandler<RequestPasswordResetCommand>
{
    public async Task Handle(RequestPasswordResetCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return;
        
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = $"https://withly.app/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email!)}";

        _ = emailService.SendAsync(
            new PasswordResetEmail
            {
                To = user.Email!,
                Username = user.Email!,
                ResetLink = resetLink
            }, ct);
    }
}