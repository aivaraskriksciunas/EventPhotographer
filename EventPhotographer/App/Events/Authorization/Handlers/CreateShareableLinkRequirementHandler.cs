using EventPhotographer.App.Events.Authorization.Requirements;
using EventPhotographer.App.Events.Entities;
using EventPhotographer.App.Events.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EventPhotographer.App.Events.Authorization.Handlers;

public class CreateShareableLinkRequirementHandler(
    EventShareableLinkService service) : EventAccessHandler<CreateShareableLinkRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        CreateShareableLinkRequirement requirement, 
        Event resource)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (IsOwner(userId, resource)
            && EventIsNotPassed(resource)
            && await EventHasExistingShareableLink(resource) == false)
        {
            context.Succeed(requirement);
        }
    }

    private bool EventIsNotPassed(Event resource)
    {
        return resource.EndDate > DateTime.UtcNow;
    }

    private async Task<bool> EventHasExistingShareableLink(Event resource)
    {
        return (await service.GetShareableLinks(resource)).Any();
    }
}
