using EventPhotographer.Core.Features.Events.Entities;
using Microsoft.AspNetCore.Authorization;

namespace EventPhotographer.App.Events.Authorization;

public class GenerateFileArchiveRequirement : IAuthorizationRequirement
{
}

public class GenerateFileArchiveRequirementHandler : EventAccessHandler<GenerateFileArchiveRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        GenerateFileArchiveRequirement requirement,
        Event resource)
    {
        if (DateTime.UtcNow > resource.EndDate)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
