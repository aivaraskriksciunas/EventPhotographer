using EventPhotographer.App.Events.Authorization.Requirements;
using EventPhotographer.App.Events.Entities;
using EventPhotographer.App.Events.Services;
using Microsoft.AspNetCore.Authorization;

namespace EventPhotographer.App.Events.Authorization.Handlers;

public class JoinEventRequirementHandler (
    EventService eventService) 
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
