using MediatR;
using Withly.Application.Auth.Dtos;
using Withly.Application.Common;

namespace Withly.Application.Auth.Commands;

public record RefreshTokenCommand(string Token) : IRequest<Result<AuthResultDto>>;