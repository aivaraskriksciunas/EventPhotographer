using EventPhotographer.App.Events.Authorization.Requirements;
using EventPhotographer.Core.Features.Events.Entities;
using Microsoft.AspNetCore.Authorization;

namespace EventPhotographer.App.Events.Authorization.Handlers;

public class UploadEventMediaRequirementHandler : EventAccessHandler<UploadEventMediaRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        UploadEventMediaRequirement requirement, 
        Event resource)
    {
        if (DateTime.UtcNow <= resource.StartDate
            || DateTime.UtcNow >= resource.EndDate.AddDays(1)) 
        {
            context.Fail();
            return Task.CompletedTask;
        }

        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}
