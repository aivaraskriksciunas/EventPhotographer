using EventPhotographer.App.Events.Entities;
using EventPhotographer.App.Events.Mappers;
using EventPhotographer.App.Events.Resources;
using EventPhotographer.Core;
using System.Security.Cryptography;

namespace EventPhotographer.App.Events.Services;

public class EventService
{
    private readonly AppDbContext db;

    public EventService(
        AppDbContext context)
    {
        db = context;
    }

    public async Task<Event?> GetById(Guid id)
    {
        return await db.Events.FindAsync(id);
    }

    public async Task<Event> CreateEvent(EventDto resource)
    {
        var entity = resource.ToEntity();
        entity.CreatedAt = DateTime.UtcNow;

        var eventDuration = Enum.Parse<EventDuration>(resource.EventDuration);
        entity.EndDate = CalculateEventEndDate(entity.StartDate, eventDuration);
        entity.AdministratorAccessKey = GenerateAdministratorAccessCode();

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

    private DateTime CalculateEventEndDate(DateTime startDate, EventDuration eventDuration)
    {
        return eventDuration switch
        {
            EventDuration.OneHour => startDate.AddHours(1),
            EventDuration.TwoHours => startDate.AddHours(2),
            EventDuration.ThreeHours => startDate.AddHours(3),
            EventDuration.SixHours => startDate.AddHours(6),
            EventDuration.TwelveHours => startDate.AddHours(12),
            EventDuration.OneDay => startDate.AddDays(1),
            EventDuration.TwoDays => startDate.AddDays(2),
            EventDuration.FiveDays => startDate.AddDays(5),
            EventDuration.OneWeek => startDate.AddDays(7),
            _ => startDate.AddHours(1),
        };
    }

    private string GenerateAdministratorAccessCode()
    {
        return RandomNumberGenerator.GetString("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-", 36);
    }
}
