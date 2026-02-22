using EventPhotographer.App.Events.Authorization.Requirements;
using EventPhotographer.App.Events.Entities;
using EventPhotographer.App.Events.Mappers;
using EventPhotographer.App.Events.Resources;
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
    public async Task<ActionResult<Event>> Get(Guid id)
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
    public async Task<ActionResult<Event>> Create(
        [FromBody]EventDto resource,
        [FromServices] IValidator<EventDto> validator)
    {
        await validator.ValidateAndThrowAsync(resource);
        var user = await userManager.GetUserAsync(User);

        var entity = await service.CreateEvent(resource, user!);

        return CreatedAtAction(
            nameof(Get), 
            new { id = entity.Id }, 
            entity
        );
    }

    [HttpPut]
    [Route("{id:guid}")]
    public async Task<ActionResult<Event>> Update(
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

        return Ok(entity);
    }

    [HttpGet]
    [Route("Durations")]
    [AllowAnonymous]
    public IActionResult GetEventDurations()
    {
        return Ok(Enum.GetNames<EventDuration>());
    }
}
