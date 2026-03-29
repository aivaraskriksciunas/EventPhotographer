using EventPhotographer.App.Content.Entities;
using EventPhotographer.App.Content.Services;
using Microsoft.AspNetCore.Authorization;

namespace EventPhotographer.App.Content.Authorization;

public class UploadFileRequirementHandler(
    MediaService mediaService) 
    : AuthorizationHandler<UploadFileRequirement, Media>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        UploadFileRequirement requirement, 
        Media resource)
    {
        if (await mediaService.HasFileAsync(resource) == true)
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
    }
}

public class UploadFileRequirement : IAuthorizationRequirement { }
