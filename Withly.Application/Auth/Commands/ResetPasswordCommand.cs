using MediatR;
using Withly.Application.Auth.Dtos;
using Withly.Application.Common;

namespace Withly.Application.Auth.Commands;

public record ResetPasswordCommand(string Email, string Token, string Password) : IRequest<Result<AuthResultDto>>;