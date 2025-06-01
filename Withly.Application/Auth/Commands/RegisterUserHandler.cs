using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Withly.Application.Auth.Dtos;
using Withly.Application.Auth.Exceptions;
using Withly.Application.Auth.Interfaces;
using Withly.Application.Common;
using Withly.Application.Common.Interfaces;
using Withly.Application.Emails.Interfaces;
using Withly.Application.Emails.Templates;
using Withly.Domain.Entities;
using Withly.Domain.Repositories;
using Withly.Persistence;

namespace Withly.Application.Auth.Commands;

[UsedImplicitly]
public class RegisterUserHandler(
    UserManager<ApplicationUser> userManager,
    IAuthTokenGenerator tokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator,
    AppDbContext dbContext,
    IBackgroundEmailQueue emailQueue,
    IUserProfileRepository userProfileRepository)
    : IRequestHandler<RegisterUserCommand, Result<AuthResultDto>>
{
    public async Task<Result<AuthResultDto>> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        if (await userManager.FindByEmailAsync(request.Email) is not null)
            return Result<AuthResultDto>.Fail("Email is already registered.");
        
        var user = new ApplicationUser { Email = request.Email, UserName = request.Email };
        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result<AuthResultDto>.Fail($"Registration failed: {errors}");
        }

        await userProfileRepository.AddAsync(new UserProfile
        {
            Id = user.Id,
            AvatarUrl = "TheLastAirbender",
            DisplayName = request.DisplayName,
        }, ct);
        
        var refreshToken = refreshTokenGenerator.Generate(user.Id); 

        dbContext.RefreshTokens.Add(refreshToken);
        await dbContext.SaveChangesAsync(ct);

        emailQueue.QueueEmail(new WelcomeEmail
        {
            To = user.Email,
            DisplayName = request.DisplayName
        });
        
        return Result<AuthResultDto>.Success(new AuthResultDto
        {
            AccessToken = tokenGenerator.Generate(user),
            RefreshToken = refreshToken.Token
        });
    }
}