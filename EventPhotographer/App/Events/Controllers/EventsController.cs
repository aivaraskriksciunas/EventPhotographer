using EventPhotographer.App.Events.Entities;
using EventPhotographer.App.Events.Resources;
using EventPhotographer.App.Events.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventPhotographer.App.Events.Controllers;

public class EventsController : ApiController
{
    private readonly EventService service;

    public EventsController(EventService service)
    {
        this.service = service;
    }

    [HttpGet]
    [Route("{id:int}")]
    public async Task<ActionResult<Event>> Get(int id)
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
        [FromBody]EventResource resource)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var entity = await service.CreateEvent(resource);

        return CreatedAtAction(nameof(Get), new { id = entity.Id }, entity);
    }

    [HttpPut]
    [Route("{id:int}")]
    public async Task<ActionResult<Event>> Update(
        int id,
        [FromBody] EventResource resource)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var entity = await service.GetById(id);
        if (entity == null)
        {
            return NotFound();
        }
        
        await service.UpdateEvent(entity, resource);

        return Ok(entity);
    }
}
