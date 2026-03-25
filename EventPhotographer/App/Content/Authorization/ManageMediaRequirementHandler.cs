using EventPhotographer.App.Content.Entities;
using EventPhotographer.App.Events.Services;
using Microsoft.AspNetCore.Authorization;

namespace EventPhotographer.App.Content.Authorization;

public class ManageMediaRequirementHandler (
    IHttpContextAccessor httpContextAccessor)
    : AuthorizationHandler<ManageMediaRequirement, Media>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        ManageMediaRequirement requirement, 
        Media resource)
    {
        var participant = httpContextAccessor.HttpContext?.GetParticipant();
        if (participant == null)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        if (resource.ParticipantId != participant.Id)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}

public class ManageMediaRequirement : IAuthorizationRequirement { }
