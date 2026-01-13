using EventPhotographer.App.Events.Entities;
using EventPhotographer.App.Events.Mappers;
using EventPhotographer.App.Events.Resources;
using EventPhotographer.Core;

namespace EventPhotographer.App.Events.Services;

public class EventService
{
    private readonly AppDbContext db;

    public EventService(
        AppDbContext context)
    {
        db = context;
    }

    public async Task<Event?> GetById(int id)
    {
        return await db.Events.FindAsync(id);
    }

    public async Task<Event> CreateEvent(EventResource resource)
    {
        var entity = resource.ToEntity();
        entity.CreatedAt = DateTime.UtcNow;

        await db.Events.AddAsync(entity);
        await db.SaveChangesAsync();

        return entity;
    }
}
