using EventPhotographer.Core.Features.Events.Entities;
using EventPhotographer.Core.Features.Events.Services;
using Microsoft.AspNetCore.Authorization;

namespace EventPhotographer.App.Events.Authorization;

public class UploadEventMediaRequirement : IAuthorizationRequirement
{
}

public class UploadEventMediaRequirementHandler(
    EventPermissionsService eventPermissionsService) 
    : EventAccessHandler<UploadEventMediaRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        UploadEventMediaRequirement requirement,
        Event resource)
    {
        if (!eventPermissionsService.CanUploadEventMedia(resource))
        {
            context.Fail();
            return Task.CompletedTask;
        }

        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}
