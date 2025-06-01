using MediatR;
using Microsoft.AspNetCore.Identity;
using Withly.Application.Emails.Interfaces;
using Withly.Application.Emails.Templates;
using Withly.Domain.Repositories;
using Withly.Persistence;

namespace Withly.Application.Auth.Commands;

public class RequestPasswordResetCommandHandler(
    UserManager<ApplicationUser> userManager,
    IBackgroundEmailQueue emailQueue,
    IUserProfileRepository userProfileRepository) : IRequestHandler<RequestPasswordResetCommand>
{
    public async Task Handle(RequestPasswordResetCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return;
        
        var userProfile = await userProfileRepository.GetByIdAsync(user.Id, ct);
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = $"https://withly.app/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email!)}";

        emailQueue.QueueEmail(new PasswordResetEmail
        {
            To = user.Email!,
            Username = userProfile!.DisplayName,
            ResetLink = resetLink
        });
    }
}