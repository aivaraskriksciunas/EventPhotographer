using EventPhotographer.Core.Features.Events.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EventPhotographer.App.Events.Authorization;

public class CreateWhatsAppMessageLinkRequirement : IAuthorizationRequirement
{
}

internal class CreateWhatsAppMessageLinkRequirementHandler(
    ) : AuthorizationHandler<CreateWhatsAppMessageLinkRequirement, EventShareableLink>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CreateWhatsAppMessageLinkRequirement requirement, 
        EventShareableLink resource)
    {
        if (resource.WhatsAppMessageLink == null 
            || resource.WhatsAppMessageLink?.Status == Core.Features.MessagingIntegrations.Enums.WhatsAppMessageLinkStatus.Failed)
        {
            context.Succeed(requirement);

            return Task.CompletedTask;
        }

        context.Fail();
        return Task.CompletedTask;
    }
}