using EventPhotographer.App.Events.Authorization.Requirements;
using EventPhotographer.App.Events.Services;
using Microsoft.AspNetCore.Authorization;
using EventPhotographer.Core.Features.Events.Entities;

namespace EventPhotographer.App.Events.Authorization.Handlers;

public class JoinEventRequirementHandler (
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
