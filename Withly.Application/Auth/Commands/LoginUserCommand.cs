using MediatR;

namespace Withly.Application.Auth.Commands;

public record LoginUserCommand(string Email, string Password) : IRequest<string>;