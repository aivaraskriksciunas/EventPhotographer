using EventPhotographer.App.Events.Services;
using EventPhotographer.Core.Features.Events.Entities;
using EventPhotographer.Core.Features.Events.Services;
using Microsoft.AspNetCore.Authorization;

namespace EventPhotographer.App.Events.Authorization;

public class JoinEventRequirement : IAuthorizationRequirement
{
}

public class JoinEventRequirementHandler(
    EventPermissionsService eventPermissionsService)
    : AuthorizationHandler<JoinEventRequirement, Event>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        JoinEventRequirement requirement,
        Event resource)
    {
        if (eventPermissionsService.CanJoinEvent(resource))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
