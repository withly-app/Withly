namespace Withly.Application.Auth.Dtos;

public class RegisterUserDto
{
    /// <summary>
    /// E-mail address
    /// </summary>
    /// <example>oscar.vugt@gmail.com</example>
    public  required string Email { get; init; }
    /// <summary>
    /// Password
    /// </summary>
    /// <example>Test123!</example>
    public required string Password { get; init; }
    
    /// <summary>
    /// Display name
    /// </summary>
    /// <example>Ovvugt</example>
    public required string DisplayName { get; init; }
}