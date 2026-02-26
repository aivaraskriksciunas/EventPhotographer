using EventPhotographer.App.Events.Authorization.Requirements;
using EventPhotographer.App.Events.Mappers;
using EventPhotographer.App.Events.DTO;
using EventPhotographer.App.Events.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventPhotographer.App.Events.Controllers;

[Route("api/Events/{id:guid}/ShareableLinks")]
public class EventShareableLinksController(
    EventShareableLinkService service,
    EventService eventService,
    IAuthorizationService authorizationService) : ApiController
{
    [HttpGet]
    [Route("")]
    public async Task<ActionResult<IEnumerable<EventShareableLinkResponseDto>>> View(Guid id)
    {
        var @event = await eventService.GetById(id);
        if (@event == null)
        {
            return NotFound();
        }
           
        var result = await authorizationService.AuthorizeAsync(User, @event, new EventAccessRequirement());
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
    public async Task<ActionResult<EventShareableLinkResponseDto>> Create(Guid id)
    {
        var @event = await eventService.GetById(id);
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
}
