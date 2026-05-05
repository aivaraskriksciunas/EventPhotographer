using EventPhotographer.App.Events.Mappers;
using EventPhotographer.App.Events.DTO;
using EventPhotographer.Core;
using EventPhotographer.Core.Features.Events.Entities;
using EventPhotographer.Core.Features.Users.Entities;

namespace EventPhotographer.App.Events.Services;

public class ApiEventService : Core.Features.Events.Services.EventService
{
    public ApiEventService(
        AppDbContext context)
        : base(context)
    { }

    public async Task<Event> CreateEvent(EventDto resource, User user)
    {
        var entity = resource.ToEntity(user);
        entity.CreatedAt = DateTime.UtcNow;

        var eventDuration = Enum.Parse<EventDuration>(resource.EventDuration);
        entity.EndDate = CalculateEventEndDate(entity.StartDate, eventDuration);

        await db.Events.AddAsync(entity);
        await db.SaveChangesAsync();

        return entity;
    }

    public async Task<Event> UpdateEvent(Event entity, EventDto resource)
    {
        entity.UpdateFromDto(resource);
        var eventDuration = Enum.Parse<EventDuration>(resource.EventDuration);
        entity.EndDate = CalculateEventEndDate(entity.StartDate, eventDuration);

        db.Events.Update(entity);
        await db.SaveChangesAsync();

        return entity;
    }

}
