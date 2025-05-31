using MediatR;

namespace Withly.Application.Auth.Commands;

public record RegisterUserCommand(string Email, string Password) : IRequest<string>;