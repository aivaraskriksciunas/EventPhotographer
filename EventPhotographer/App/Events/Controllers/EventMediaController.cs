using EventPhotographer.App.Content.DTO;
using EventPhotographer.App.Content.Services;
using EventPhotographer.App.Events.Services;
using EventPhotographer.App.Events.Authorization.Requirements;
using EventPhotographer.App.Events.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EventPhotographer.App.Events.DTO.Response;

namespace EventPhotographer.App.Events.Controllers;

[Route("api/Events/{eventId:guid}/Media")]
public class EventMediaController(
    EventService eventService,
    MediaService mediaService,
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
}
