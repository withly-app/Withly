using MediatR;
using Withly.Application.Common;

namespace Withly.Application.Auth.Commands;
public record RequestPasswordResetCommand(string Email) : IRequest;

