using MediatR;
using Withly.Application.Auth.Dtos;
using Withly.Application.Common;

namespace Withly.Application.Auth.Commands;

public record RegisterUserCommand(string Email, string Password, string DisplayName) : IRequest<Result<AuthResultDto>>;