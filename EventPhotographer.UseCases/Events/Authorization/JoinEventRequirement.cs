using EventPhotographer.Core.Features.Events.Entities;
using EventPhotographer.UseCases.Common.Authorization;

namespace EventPhotographer.UseCases.Events.Authorization;

public class JoinEventRequirement : IAuthorizationRequirement {}

internal class JoinEventRequirementHandler : ResourceAuthorizationHandler<Event, JoinEventRequirement>
{
    public override async Task<AuthorizationResult> HandleAsync(Event entity, JoinEventRequirement requirement)
    {
        if (!entity.IsActive())
        {
            return AuthorizationResult.Failure();
        }

        return AuthorizationResult.Success();
    }
}