using EventPhotographer.App.Events.Authorization.Requirements;
using EventPhotographer.App.Events.Services;
using Microsoft.AspNetCore.Authorization;
using EventPhotographer.Core.Features.Events.Entities;

namespace EventPhotographer.App.Events.Authorization.Handlers;

public class IsActiveParticipantRequirementHandler (
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
