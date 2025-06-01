using MediatR;
using Withly.Application.UserProfiles.Dtos;

namespace Withly.Application.UserProfiles.Queries;

public record GetUserProfileQuery(Guid UserId) : IRequest<UserProfileDto?>;