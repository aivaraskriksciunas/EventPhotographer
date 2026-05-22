using EventPhotographer.App.Events.Services;
using EventPhotographer.Core.Features.Events.Entities;
using Microsoft.AspNetCore.Authorization;

namespace EventPhotographer.App.Events.Authorization;

public class JoinEventRequirement : IAuthorizationRequirement
{
}

public class JoinEventRequirementHandler(
    ApiEventService eventService)
    : AuthorizationHandler<JoinEventRequirement, Event>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        JoinEventRequirement requirement,
        Event resource)
    {
        if (eventService.IsEventActive(resource))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
