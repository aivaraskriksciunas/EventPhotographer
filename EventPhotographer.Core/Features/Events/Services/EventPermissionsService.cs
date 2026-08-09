using EventPhotographer.Core.Features.Events.Entities;

namespace EventPhotographer.Core.Features.Events.Services;

public class EventPermissionsService
{
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
