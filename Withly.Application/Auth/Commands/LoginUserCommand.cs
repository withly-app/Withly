using MediatR;
using Withly.Application.Auth.Dtos;
using Withly.Application.Common;

namespace Withly.Application.Auth.Commands;

public record LoginUserCommand(string Email, string Password) : IRequest<Result<AuthResultDto>>;