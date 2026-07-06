using EventPhotographer.Core.Features.Users.Entities;
using EventPhotographer.UseCases.Common.Authorization;

namespace EventPhotographer.UseCases.Events.Authorization;

public class CreateEventRequirement : IAuthorizationRequirement {}

internal class CreateEventRequirementHandler : AuthorizationHandler<CreateEventRequirement>
{
    public override async Task<AuthorizationResult> HandleAsync(User user, CreateEventRequirement requirement)
    {
        return AuthorizationResult.Success();
    }
}