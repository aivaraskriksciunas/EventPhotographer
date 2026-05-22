using EventPhotographer.App.Events.Services;
using EventPhotographer.Core.Features.Events.Entities;
using Microsoft.AspNetCore.Authorization;

namespace EventPhotographer.App.Events.Authorization;

public class IsActiveParticipantRequirement : IAuthorizationRequirement
{
}

public class IsActiveParticipantRequirementHandler(
    IHttpContextAccessor httpContextAccessor)
    : AuthorizationHandler<IsActiveParticipantRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        IsActiveParticipantRequirement requirement)
    {
        var httpContext = httpContextAccessor.HttpContext;

        if (httpContext?.GetParticipant() is Participant)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }

        return Task.CompletedTask;
    }
}
