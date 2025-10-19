using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Withly.Application.Auth.Dtos;
using Withly.Application.Auth.Interfaces;
using Withly.Application.Common;
using Withly.Application.Common.Interfaces;
using Withly.Application.Emails.Templates;
using Withly.Application.UserProfiles.Dtos;
using Withly.Domain.Exceptions;
using Withly.Persistence;
using Withly.Persistence.Entities;
using Withly.Persistence.Entities.Interfaces;

namespace Withly.Application.Services;

public class UserService(
    ILogger<UserService> logger,
    UserManager<ApplicationUser> userManager,
    IAuthTokenGenerator tokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator,
    AppDbContext appDbContext,
    IBackgroundEmailQueue emailQueue,
    SignInManager<ApplicationUser> signInManager)
{
    public async Task<Result<AuthResultDto>> RegisterAsync(RegisterUserDto dto, CancellationToken ct)
    {
        if (await userManager.FindByEmailAsync(dto.Email) is not null)
        {
            return Result<AuthResultDto>.Fail("Email is already registered.");
        }

        var user = new ApplicationUser { Email = dto.Email, UserName = dto.Email };
        var result = await userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result<AuthResultDto>.Fail($"Registration failed: {errors}");
        }

        await appDbContext.UserProfiles.AddAsync(new UserProfile
        {
            Id = user.Id,
            AvatarUrl = "TheLastAirbender",
            DisplayName = dto.DisplayName,
        }, ct);

        var refreshToken = refreshTokenGenerator.Generate(user.Id);

        await appDbContext.RefreshTokens.AddAsync(refreshToken, ct);
        await appDbContext.SaveChangesAsync(ct);

        emailQueue.QueueEmail(new WelcomeEmail
        {
            To = user.Email,
            DisplayName = dto.DisplayName
        }, user.Id);

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

        await appDbContext.RefreshTokens.AddAsync(refreshToken, ct);
        await appDbContext.SaveChangesAsync(ct);

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
        {
            return;
        }

        var userProfile = await appDbContext.UserProfiles
            .AsNoTracking()
            .Where(profile => profile.Id == user.Id)
            .FirstAsync(ct);

        var token = await userManager.GeneratePasswordResetTokenAsync(user);

        var resetLink =
            $"https://withly.app/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";

        emailQueue.QueueEmail(new PasswordResetEmail
        {
            To = user.Email,
            Username = userProfile.DisplayName,
            ResetLink = resetLink
        }, user.Id);
    }

    public async Task<Result<AuthResultDto>> ResetPasswordAsync(string email, string token, string newPassword,
        CancellationToken ct)
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


        var tokens = appDbContext.RefreshTokens.Where(rft => rft.UserId == user.Id);
        appDbContext.RefreshTokens.RemoveRange(tokens);

        var refreshToken = refreshTokenGenerator.Generate(user.Id);

        await appDbContext.RefreshTokens.AddAsync(refreshToken, ct);
        await appDbContext.SaveChangesAsync(ct);

        return Result<AuthResultDto>.Success(new AuthResultDto
        {
            AccessToken = tokenGenerator.Generate(user),
            RefreshToken = refreshToken.Token
        });
    }

    public async Task<UserProfileDto> GetUserByIdAsync(Guid id, CancellationToken ct)
    {
        var profile = await appDbContext.UserProfiles.AsNoTracking().Where(profile => profile.Id == id)
            .FirstOrDefaultAsync(ct);

        if (profile is null) throw EntityNotFoundException.For<UserProfile>(id.ToString());

        return new UserProfileDto(
            profile.Id,
            profile.DisplayName,
            profile.AvatarUrl
        );
    }

    public async Task<UserProfileDto?> FindUserByEmailAsync(string email, CancellationToken ct)
    {
        var userWithProfile = await appDbContext.Users
            .Where(user => user.Email == email)
            .Include(x => x.Profile)
            .FirstOrDefaultAsync(ct);

        if (userWithProfile is null) return null;

        return new UserProfileDto(
            userWithProfile.Id,
            userWithProfile.Profile.DisplayName,
            userWithProfile.Profile.AvatarUrl
        );
    }
}