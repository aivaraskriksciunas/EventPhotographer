using EventPhotographer.App.Events.Authorization.Requirements;
using EventPhotographer.App.Events.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EventPhotographer.App.Events.Authorization.Handlers;

public class EventAuthorizationHandler : AuthorizationHandler<EventAccessRequirement, Event>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        EventAccessRequirement requirement, 
        Event resource)
    {
        if (resource.UserId == context.User.FindFirstValue(ClaimTypes.NameIdentifier))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
