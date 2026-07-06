using EventPhotographer.Core.Features.Events.Entities;
using EventPhotographer.Core.Features.Users.Entities;

namespace EventPhotographer.Core.Features.Events.Services;

public class EventPermissionsService
{
    public bool IsOwner(string userId, Event @event)
    {
        return @event.UserId?.ToString() == userId;
    }

    public bool CanJoinEvent(Event @event)
    {
        return @event.IsActive();
    }

    public bool CanUploadEventMedia(Event @event)
    {
        if (DateTime.UtcNow <= @event.StartDate
            || DateTime.UtcNow >= @event.EndDate.AddDays(1))
        {
            return false;
        }

        return true;
    }
}
