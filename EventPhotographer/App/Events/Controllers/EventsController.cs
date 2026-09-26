using EventPhotographer.App.Events.DTO;
using EventPhotographer.App.Events.DTO.Response;
using EventPhotographer.App.Events.Mappers;
using EventPhotographer.Core.Extensions;
using EventPhotographer.Core.Features.Events.Entities;
using EventPhotographer.Core.Features.Users.Entities;
using EventPhotographer.UseCases.Common.Commands;
using EventPhotographer.UseCases.Common.Queries;
using EventPhotographer.UseCases.Events;
using EventPhotographer.UseCases.Events.Commands;
using EventPhotographer.UseCases.Events.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EventPhotographer.App.Events.Controllers;

public class EventsController(
    EventQueryService queryService,
    UserManager<User> userManager) 
    : ApiController
{
    [HttpGet]
    [Route("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<EventResponseDto>> Get(Guid id)
    {
        var user = await userManager.GetUserAsync(User);
        var result = await queryService.GetEventAsync(id, user!);
        if (!result.IsSuccess)
        {
            return result.ToProblemDetailsResult();
        }

        return Ok(EventMapper.CreateResponseDto(result.Value));
    }

    [HttpGet]
    [Route("")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<EventResponseDto>>> GetAll(
        [FromServices] IQueryHandler<GetUserEventsQuery, IEnumerable<EventListModel>> queryHandler)
    {
        var user = await userManager.GetUserAsync(User);
        var result = await queryHandler.QueryAsync(new GetUserEventsQuery
        {
            User = user!,
        });

        if (!result.IsSuccess)
        {
            return result.ToProblemDetailsResult();
        }

        return Ok(result.Value.Select(e => new EventResponseDto
        {
            Id = e.Id,
            Name = e.Name,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            ParticipantCount = e.ParticipantCount,
        }));
    }

    [HttpPost]
    [Route("")]
    [Authorize]
    public async Task<ActionResult<EventResponseDto>> Create(
        [FromBody] EventDto resource,
        [FromServices] ICommandHandler<CreateEvent, Event> commandHandler)
    {
        var user = await userManager.GetUserAsync(User);
        var result = await commandHandler.HandleAsync(new CreateEvent
        {
            Name = resource.Name,
            StartDate = resource.StartDate,
            EventDuration = resource.EventDuration,
            User = user!
        });

        if (!result.IsSuccess)
        {
            return result.ToProblemDetailsResult();
        }

        return CreatedAtAction(
            nameof(Get),
            new { id = result.Value.Id },
            EventMapper.CreateResponseDto(result.Value)
        );
    }

    [HttpPut]
    [Route("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<EventResponseDto>> Update(
        Guid id,
        [FromBody] EventDto resource,
        [FromServices] ICommandHandler<UpdateEvent, Event> commandHandler)
    {
        var user = await userManager.GetUserAsync(User);
        var entityResult = await queryService.GetEventAsync(id, user!);
        if (!entityResult.IsSuccess)
        {
            return entityResult.ToProblemDetailsResult();
        }

        var result = await commandHandler.HandleAsync(new UpdateEvent
        {
            Event = entityResult.Value,
            Name = resource.Name,
            StartDate = resource.StartDate,
            EventDuration = resource.EventDuration,
            User = user!
        });

        if (!result.IsSuccess)
        {
            return result.ToProblemDetailsResult();
        }

        return Ok(EventMapper.CreateResponseDto(result.Value));
    }

    [HttpGet]
    [Route("Durations")]
    public IActionResult GetEventDurations()
    {
        return Ok(Enum.GetNames<EventDuration>());
    }
}
