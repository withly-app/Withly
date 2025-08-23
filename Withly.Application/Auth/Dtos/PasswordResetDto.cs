namespace Withly.Application.Auth.Dtos;

public record PasswordResetDto(string Email, string Token, string NewPassword);