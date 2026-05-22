using EventPhotographer.App.Content.Services;
using EventPhotographer.App.Events.Services;
using EventPhotographer.App.Events.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EventPhotographer.App.Events.DTO.Response;

namespace EventPhotographer.App.Events.Controllers;

[Route("api/Events/{eventId:guid}/Media")]
public class EventMediaController(
    ApiEventService eventService,
    ApiMediaService mediaService,
    IAuthorizationService authorizationService) : ApiController
{
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<EventMediaResponseDto>>> List(
        Guid eventId)
    {
        var @event = await eventService.GetByIdAsync(eventId);
        if (@event == null)
        {
            return NotFound();
        }

        var result = await authorizationService.AuthorizeAsync(
            User, @event, new ManageEventRequirement());
        if (!result.Succeeded)
        {
            return NotFound();
        }

        return Ok(await mediaService.GetForEventAsync(@event));
    }

    [HttpGet("Archives")]
    [Authorize]
    public async Task<ActionResult<EventMediaResponseDto?>> ListArchives(
        Guid eventId)
    {
        var @event = await eventService.GetByIdAsync(eventId);
        if (@event == null)
        {
            return NotFound();
        }

        var result = await authorizationService.AuthorizeAsync(
            User, @event, new ManageEventRequirement());
        if (!result.Succeeded)
        {
            return NotFound();
        }

        return Ok(await mediaService.GetArchiveForEventAsync(@event));
    }
}
