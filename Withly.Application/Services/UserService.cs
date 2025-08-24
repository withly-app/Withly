using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Withly.Application.Auth.Dtos;
using Withly.Application.Auth.Interfaces;
using Withly.Application.Common;
using Withly.Application.Common.Interfaces;
using Withly.Application.Emails.Templates;
using Withly.Application.UserProfiles.Dtos;
using Withly.Domain.Entities;
using Withly.Domain.Repositories;
using Withly.Infrastructure.Models.Email.Interfaces;
using Withly.Persistence;

namespace Withly.Application.Services;

public class UserService(
    UserManager<ApplicationUser> userManager,
    IAuthTokenGenerator tokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator,
    IUnitOfWork unitOfWork,
    IBackgroundEmailQueue emailQueue,
    IUserProfileRepository userProfileRepository,
    IRefreshTokenRepository refreshTokenRepository,
    SignInManager<ApplicationUser> signInManager,
    ILogger<UserService> logger)
{
    public async Task<Result<AuthResultDto>> RegisterAsync(RegisterUserDto dto, CancellationToken ct)
    {
        if (await userManager.FindByEmailAsync(dto.Email) is not null)
            return Result<AuthResultDto>.Fail("Email is already registered.");
        
        var user = new ApplicationUser { Email = dto.Email, UserName = dto.Email };
        var result = await userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result<AuthResultDto>.Fail($"Registration failed: {errors}");
        }

        await userProfileRepository.AddAsync(new UserProfile
        {
            Id = user.Id,
            AvatarUrl = "TheLastAirbender",
            DisplayName = dto.DisplayName,
        }, ct);
        
        var refreshToken = refreshTokenGenerator.Generate(user.Id); 

        await refreshTokenRepository.AddAsync(refreshToken, ct);
        await unitOfWork.SaveChangesAsync(ct);

        emailQueue.QueueEmail(new WelcomeEmail
        {
            To = user.Email,
            DisplayName = dto.DisplayName
        });
        
        return Result<AuthResultDto>.Success(new AuthResultDto
        {
            AccessToken = tokenGenerator.Generate(user),
            RefreshToken = refreshToken.Token
        });
    }
    public async Task<Result<AuthResultDto>> LoginAsync(string email, string password, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null) return Result<AuthResultDto>.Fail("Invalid credentials");

        var result = await signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!result.Succeeded) return Result<AuthResultDto>.Fail("Invalid credentials");

        var refreshToken = refreshTokenGenerator.Generate(user.Id); 

        await refreshTokenRepository.AddAsync(refreshToken, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result<AuthResultDto>.Success(new AuthResultDto
        {
            AccessToken = tokenGenerator.Generate(user),
            RefreshToken = refreshToken.Token
        });
    }
    
    public async Task RequestPasswordResetAsync(string email, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(email);
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
    
    public async Task<Result<AuthResultDto>> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            logger.LogWarning("User not found, probably forged request");
            return Result<AuthResultDto>.Fail("User not found");
        }

        var result = await userManager.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded)
            return Result<AuthResultDto>.Fail(string.Join("; ", result.Errors.Select(e => e.Description)));

        refreshTokenRepository.RemoveTokensForUser(user.Id);
        
        var refreshToken = refreshTokenGenerator.Generate(user.Id);

        await refreshTokenRepository.AddAsync(refreshToken, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result<AuthResultDto>.Success(new AuthResultDto
        {
            AccessToken = tokenGenerator.Generate(user),
            RefreshToken = refreshToken.Token
        });
    }
    public async Task<UserProfileDto?> GetUserByIdAsync(Guid id, CancellationToken ct)
    {
        var profile = await userProfileRepository.GetByIdAsync(id, ct);
        if (profile is null) return null;

        return new UserProfileDto(
            profile.Id,
            profile.DisplayName,
            profile.AvatarUrl
        );
    }
}