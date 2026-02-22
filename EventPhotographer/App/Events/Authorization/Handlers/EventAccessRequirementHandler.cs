using EventPhotographer.App.Events.Authorization.Requirements;
using EventPhotographer.App.Events.Entities;
using EventPhotographer.App.Users.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EventPhotographer.App.Events.Authorization.Handlers;

public class EventAccessRequirementHandler : EventAccessHandler<EventAccessRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        EventAccessRequirement requirement, 
        Event resource)
    {
        if (IsOwner(context.User.FindFirstValue(ClaimTypes.NameIdentifier), resource))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
