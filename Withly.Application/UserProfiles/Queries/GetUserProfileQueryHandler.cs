using MediatR;
using Withly.Application.UserProfiles.Dtos;
using Withly.Domain.Repositories;

namespace Withly.Application.UserProfiles.Queries;

public class GetUserProfileQueryHandler(IUserProfileRepository repository) : IRequestHandler<GetUserProfileQuery, UserProfileDto?>
{
    public async Task<UserProfileDto?> Handle(GetUserProfileQuery request, CancellationToken ct)
    {
        var profile = await repository.GetByIdAsync(request.UserId, ct);
        if (profile is null) return null;

        return new UserProfileDto(
            profile.Id,
            profile.DisplayName,
            profile.AvatarUrl
        );
    }
}