using EventPhotographer.Core;
using EventPhotographer.Core.Features.Events.Entities;
using EventPhotographer.Core.Features.Users.Entities;
using EventPhotographer.UseCases.Common;
using EventPhotographer.UseCases.Common.Authorization;
using EventPhotographer.UseCases.Events.Authorization;
using Microsoft.EntityFrameworkCore;

namespace EventPhotographer.UseCases.Events;

public class EventQueryService(
    AppDbContext dbContext,
    AuthorizationService authorizationService)
{
    public async Task<Result<Event>> GetEventAsync(Guid id, User user)
    {
        var @event = await dbContext.Events.FindAsync(id);
        if (@event is null)
        {
            return Error.NotFound;
        }

        var authResult = await authorizationService.AuthorizeAsync(user, @event, new ManageEventRequirement());
        if (!authResult.IsAuthorized)
        {
            return authResult;
        }

        return @event;
    }

    public async Task<Result<IEnumerable<Event>>> GetAllForUserAsync(User user)
    {
        var events = await dbContext.Events
            .Where(e => e.UserId == user.Id)
            .OrderByDescending(e => e.StartDate)
            .ToListAsync();

        return events;
    }
}
