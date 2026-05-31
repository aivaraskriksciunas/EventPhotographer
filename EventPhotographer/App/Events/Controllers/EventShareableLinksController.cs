using EventPhotographer.App.Events.Authorization;
using EventPhotographer.App.Events.Mappers;
using EventPhotographer.App.Events.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EventPhotographer.App.Events.DTO.Response;
using EventPhotographer.App.MessagingIntegrations.DTO.Response;
using EventPhotographer.Core.Features.MessagingIntegrations.Services;
using EasyNetQ;
using EventPhotographer.Core.Features.MessagingIntegrations.Messages;
using EventPhotographer.App.MessagingIntegrations.Mappers;

namespace EventPhotographer.App.Events.Controllers;

[Route("api/Events/{id:guid}/ShareableLinks")]
public class EventShareableLinksController(
    EventShareableLinkService service,
    ApiEventService eventService,
    IAuthorizationService authorizationService) : ApiController
{
    [HttpGet]
    [Route("")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<EventShareableLinkResponseDto>>> View(Guid id)
    {
        var @event = await eventService.GetByIdAsync(id);
        if (@event == null)
        {
            return NotFound();
        }
           
        var result = await authorizationService.AuthorizeAsync(User, @event, new ManageEventRequirement());
        if (!result.Succeeded)
        {
            return Forbid();
        }

        var shareableLink = await service.GetShareableLinks(@event);
        if (shareableLink == null)
        {
            return Ok();
        }

        return Ok(shareableLink.Select(l => EventShareableLinkMapper.CreateResponseDto(l)).ToArray());
    }

    [HttpPost]
    [Route("")]
    [Authorize]
    public async Task<ActionResult<EventShareableLinkResponseDto>> Create(Guid id)
    {
        var @event = await eventService.GetByIdAsync(id);
        if (@event == null)
        {
            return NotFound();
        }

        var result = await authorizationService.AuthorizeAsync(User, @event, new CreateShareableLinkRequirement());
        if (!result.Succeeded)
        {
            return Forbid();
        }

        var shareableLink = await service.CreateShareableLink(@event);

        return Ok(EventShareableLinkMapper.CreateResponseDto(shareableLink));
    }

    [HttpPost]
    [Route("{linkId:guid}/WhatsApp")]
    [Authorize]
    public async Task<ActionResult<WhatsAppMessageLinkResponseDto>> CreateWhatsAppMessageLink(
        Guid id,
        Guid linkId,
        [FromServices] WhatsAppMessageLinkService messageLinkService,
        [FromServices] IBus bus)
    {
        var shareableLink = await service.GetByIdAsync(linkId);
        if (shareableLink == null || shareableLink.EventId != id)
        {
            return NotFound();
        }

        var eventResult = await authorizationService.AuthorizeAsync(User, shareableLink.Event, new ManageEventRequirement());
        var authResult = await authorizationService.AuthorizeAsync(User, shareableLink, new CreateWhatsAppMessageLinkRequirement());
        if (!authResult.Succeeded || !eventResult.Succeeded) {
            return Forbid();
        }

        var whatsAppLink = await messageLinkService.CreatePendingAsync(shareableLink);
        await bus.PubSub.PublishAsync(new CreateWhatsAppMessageLink { WhatsAppMessageLinkId = whatsAppLink.Id });

        return Ok(whatsAppLink.ToResponseDto());
    }
}
