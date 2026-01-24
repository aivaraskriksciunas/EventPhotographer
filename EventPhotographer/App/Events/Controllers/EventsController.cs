using EventPhotographer.App.Events.Entities;
using EventPhotographer.App.Events.Mappers;
using EventPhotographer.App.Events.Resources;
using EventPhotographer.App.Events.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace EventPhotographer.App.Events.Controllers;

public class EventsController : ApiController
{
    private readonly EventService service;

    public EventsController(EventService service)
    {
        this.service = service;
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<ActionResult<Event>> Get(Guid id)
    {
        var entity = await service.GetById(id);
        
        if (entity == null)
        {
            return NotFound();
        }

        return Ok(EventMapper.CreateResponseDto(entity));
    }

    [HttpPost]
    [Route("")]
    public async Task<ActionResult<Event>> Create(
        [FromBody]EventDto resource,
        [FromServices] IValidator<EventDto> validator)
    {
        await validator.ValidateAndThrowAsync(resource);

        var entity = await service.CreateEvent(resource);

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
        
        await service.UpdateEvent(entity, resource);

        return Ok(entity);
    }

    [HttpGet]
    [Route("Durations")]
    public IActionResult GetEventDurations()
    {
        return Ok(Enum.GetNames<EventDuration>());
    }
}
