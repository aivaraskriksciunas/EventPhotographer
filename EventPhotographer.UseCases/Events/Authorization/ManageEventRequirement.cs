using EventPhotographer.Core.Features.Events.Entities;
using EventPhotographer.Core.Features.Users.Entities;
using EventPhotographer.UseCases.Common.Authorization;

namespace EventPhotographer.UseCases.Events.Authorization;

public class ManageEventRequirement : IAuthorizationRequirement {}

internal class ManageEventRequirementHandler : ResourceAuthorizationHandler<Event, ManageEventRequirement>
{
    public override async Task<AuthorizationResult> HandleAsync(User user, Event entity, ManageEventRequirement requirement)
    {
        if (user.Id != entity.UserId)
        {
            return AuthorizationResult.Failure();
        }

        return AuthorizationResult.Success();
    }
}
