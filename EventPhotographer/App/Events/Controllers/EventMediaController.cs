using EventPhotographer.App.Events.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EventPhotographer.UseCases.Common.Queries;
using EventPhotographer.UseCases.Content.Queries;
using Microsoft.AspNetCore.Identity;
using EventPhotographer.Core.Features.Users.Entities;
using EventPhotographer.Core.Extensions;
using EventPhotographer.App.Common.DTO.QueryParams;

namespace EventPhotographer.App.Events.Controllers;

[Route("api/Events/{eventId:guid}/Media")]
public class EventMediaController(
    ApiEventService eventService,
    UserManager<User> userManager) : ApiController
{
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<PagedResult<MediaModel>>> List(
        Guid eventId,
        [FromQuery] PaginationQueryParameters parameters,
        [FromServices] IQueryHandler<GetMediaListForEventQuery, PagedResult<MediaModel>> queryHandler)
    {
        var @event = await eventService.GetByIdAsync(eventId);
        var user = await userManager.GetUserAsync(User);
        if (@event == null)
        {
            return NotFound();
        }

        var result = await queryHandler.QueryAsync(new GetMediaListForEventQuery
        {
            Event = @event,
            User = user!,
            Page = parameters.Page,
            PageSize = parameters.PageSize,
        });
        if (!result.IsSuccess)
        {
            return result.ToProblemDetailsResult();
        }

        return Ok(result.Value);
    }

    [HttpGet("Archives")]
    [Authorize]
    public async Task<ActionResult<MediaModel?>> ListArchives(
        Guid eventId,
        [FromServices] IQueryHandler<GetArchiveForEventQuery, MediaModel?> queryHandler)
    {
        var @event = await eventService.GetByIdAsync(eventId);
        var user = await userManager.GetUserAsync(User);
        if (@event == null)
        {
            return NotFound();
        }

        var result = await queryHandler.QueryAsync(new GetArchiveForEventQuery
        {
            Event = @event,
            User = user!
        });
        if (!result.IsSuccess)
        {
            return result.ToProblemDetailsResult();
        }

        return Ok(result.Value);
    }
}
