using EventPhotographer.App.Events.Authorization.Requirements;
using EventPhotographer.App.Events.Entities;
using EventPhotographer.App.Events.Mappers;
using EventPhotographer.App.Events.DTO;
using EventPhotographer.App.Events.Services;
using EventPhotographer.App.Users.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EventPhotographer.App.Events.Controllers;

public class EventsController(
    EventService service,
    UserManager<User> userManager,
    IAuthorizationService authorizationService) : ApiController
{
    [HttpGet]
    [Route("{id:guid}")]
    public async Task<ActionResult<EventResponseDto>> Get(Guid id)
    {
        var entity = await service.GetById(id);
        if (entity == null)
        {
            return NotFound();
        }

        var result = await authorizationService.AuthorizeAsync(User, entity, new EventAccessRequirement());
        if (!result.Succeeded)
        {
            return NotFound();
        }

        return Ok(EventMapper.CreateResponseDto(entity));
    }

    [HttpGet]
    [Route("")]
    public async Task<IEnumerable<EventResponseDto>> GetAll()
    {
        var user = await userManager.GetUserAsync(User);
        var entities = await service.GetAllForUser(user!);

        return entities.CreateResponseDtos();
    }

    [HttpPost]
    [Route("")]
    public async Task<ActionResult<EventResponseDto>> Create(
        [FromBody]EventDto resource,
        [FromServices] IValidator<EventDto> validator)
    {
        await validator.ValidateAndThrowAsync(resource);
        var user = await userManager.GetUserAsync(User);

        var entity = await service.CreateEvent(resource, user!);

        return CreatedAtAction(
            nameof(Get), 
            new { id = entity.Id }, 
            EventMapper.CreateResponseDto(entity)
        );
    }

    [HttpPut]
    [Route("{id:guid}")]
    public async Task<ActionResult<EventResponseDto>> Update(
        Guid id,
        [FromBody] EventDto resource,
        [FromServices] IValidator<EventDto> validator)
    {
        await validator.ValidateAndThrowAsync(resource);

        var entity = await service.GetById(id);
        if (entity == null)
        {
            return NotFound();
        }

        var result = await authorizationService.AuthorizeAsync(User, entity, new EventAccessRequirement());
        if (!result.Succeeded)
        {
            return NotFound();
        }

        await service.UpdateEvent(entity, resource);

        return Ok(EventMapper.CreateResponseDto(entity));
    }

    [HttpPost]
    [Route("Join")]
    [AllowAnonymous]
    public async Task<ActionResult<JoinEventResponseDto>> Join(
        [FromBody] JoinEventRequestDto resource,
        [FromServices] EventShareableLinkService shareableLinkService)
    {
        var shareableLink = await shareableLinkService.GetShareableLinkByCode(resource.Code);
        if (shareableLink == null)
        {
            return NotFound();
        }

        var authResult = await authorizationService.AuthorizeAsync(User, shareableLink.Event, new JoinEventRequirement());
        if (!authResult.Succeeded)
        {
            return NotFound();
        }

        return Ok(EventMapper.CreateJoinEventResponseDto(shareableLink.Event!));
    }

    [HttpGet]
    [Route("Durations")]
    [AllowAnonymous]
    public IActionResult GetEventDurations()
    {
        return Ok(Enum.GetNames<EventDuration>());
    }
}
