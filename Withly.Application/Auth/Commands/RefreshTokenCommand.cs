using MediatR;
using Withly.Application.Auth.Dtos;

namespace Withly.Application.Auth.Commands;

public record RefreshTokenCommand(string Token) : IRequest<AuthResultDto>;