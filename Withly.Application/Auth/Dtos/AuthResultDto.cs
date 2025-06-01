namespace Withly.Application.Auth.Dtos;

public class AuthResultDto
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}