using EventPhotographer.Core.Features.Events.Entities;
using EventPhotographer.Core.Features.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventPhotographer.Core.Features.Events.Services;

public class EventService
{
    protected readonly AppDbContext db;

    public EventService(
        AppDbContext context)
    {
        db = context;
    }

    public async Task<Event?> GetByIdAsync(Guid id)
    {
        return await db.Events.FindAsync(id);
    }

    public async Task<IEnumerable<Event>> GetAllForUser(User user)
    {
        return await db.Events
            .Where(e => e.UserId == user.Id)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    protected DateTime CalculateEventEndDate(DateTime startDate, EventDuration eventDuration)
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
}
