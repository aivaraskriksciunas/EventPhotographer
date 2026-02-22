using EventPhotographer.App.Events.Authorization.Requirements;
using EventPhotographer.App.Events.Entities;
using EventPhotographer.App.Events.Mappers;
using EventPhotographer.App.Events.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventPhotographer.App.Events.Controllers;

public class EventShareableLinkController(
    EventShareableLinkService service,
    EventService eventService,
    IAuthorizationService authorizationService) : ApiController
{
    [HttpGet]
    [Route("{id:guid}")]
    public async Task<ActionResult<Event>> Create(Guid id)
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
