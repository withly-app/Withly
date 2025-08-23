using MediatR;
using Swashbuckle.AspNetCore.Annotations;
using Withly.Application.Auth.Dtos;
using Withly.Application.Common;

namespace Withly.Application.Auth.Commands;

public class RegisterUserCommand : IRequest<Result<AuthResultDto>>
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