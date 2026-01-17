using EventPhotographer.App.Events.Entities;
using EventPhotographer.App.Events.Resources;
using EventPhotographer.App.Events.Services;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace EventPhotographer.App.Events.Controllers;

public class EventsController : ApiController
{
    private readonly EventService service;
    private readonly IValidator<EventDto> validator;

    public EventsController(
        EventService service,
        IValidator<EventDto> validator)
    {
        this.service = service;
        this.validator = validator;
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

        return Ok(entity);
    }

    [HttpPost]
    [Route("")]
    public async Task<ActionResult<Event>> Create(
        [FromBody]EventDto resource)
    {
        var result = await validator.ValidateAsync(resource);
        if (!result.IsValid)
        {
            return BadRequest(result.ToDictionary());
        }

        var entity = await service.CreateEvent(resource);

        return CreatedAtAction(nameof(Get), new { id = entity.Id }, entity);
    }

    [HttpPut]
    [Route("{id:guid}")]
    public async Task<ActionResult<Event>> Update(
        Guid id,
        [FromBody] EventDto resource)
    {
        var result = await validator.ValidateAsync(resource);
        if (!result.IsValid)
        {
            return BadRequest(result.ToDictionary());
        }

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
