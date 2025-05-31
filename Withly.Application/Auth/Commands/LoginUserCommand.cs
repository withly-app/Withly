using MediatR;
using Withly.Application.Auth.Dtos;

namespace Withly.Application.Auth.Commands;

public record LoginUserCommand(string Email, string Password) : IRequest<AuthResultDto>;