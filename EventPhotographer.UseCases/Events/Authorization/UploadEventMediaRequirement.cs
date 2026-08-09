using EventPhotographer.Core.Features.Events.Entities;
using EventPhotographer.UseCases.Common.Authorization;

namespace EventPhotographer.UseCases.Events.Authorization;

public class UploadEventMediaRequirement : IAuthorizationRequirement
{
}

internal class UploadEventMediaRequirementHandler
    : ResourceAuthorizationHandler<Event, UploadEventMediaRequirement>
{
    public override async Task<AuthorizationResult> HandleAsync(Event @event, UploadEventMediaRequirement requirement)
    {
        if (DateTime.UtcNow <= @event.StartDate
            || DateTime.UtcNow >= @event.EndDate.AddDays(1))
        {
            return AuthorizationResult.Failure();
        }

        return AuthorizationResult.Success();
    }
}