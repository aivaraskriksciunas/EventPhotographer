using EventPhotographer.Core.Features.Events.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EventPhotographer.App.Events.Authorization;

public class ManageEventRequirement : IAuthorizationRequirement
{
}

public class ManageEventRequirementHandler : EventAccessHandler<ManageEventRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ManageEventRequirement requirement,
        Event resource)
    {
        if (IsOwner(context.User.FindFirstValue(ClaimTypes.NameIdentifier), resource))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
