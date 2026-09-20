using EventPhotographer.Core;
using EventPhotographer.Core.Features.Users.Entities;
using EventPhotographer.UseCases.Common;
using EventPhotographer.UseCases.Common.Queries;
using Microsoft.EntityFrameworkCore;

namespace EventPhotographer.UseCases.Events.Queries;

public record GetUserEventsQuery : IQuery<IEnumerable<EventListModel>>
{
    public required User User;
}

public record EventListModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public int ParticipantCount { get; init; }
}

public class EventQueryHandler(AppDbContext db)
    : IQueryHandler<GetUserEventsQuery, IEnumerable<EventListModel>>
{
    public async Task<Result<IEnumerable<EventListModel>>> QueryAsync(GetUserEventsQuery query, CancellationToken cancellationToken = default)
    {
        var events = await db.Events
            .Where(e => e.UserId == query.User.Id)
            .OrderByDescending(e => e.StartDate)
            .ThenBy(e => e.Id)
            .Select(e => new EventListModel
            {
                Id = e.Id,
                Name = e.Name,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                ParticipantCount = e.Participants.Count(p => p.HasUploaded),
            })
            .ToListAsync(cancellationToken);

        return events;
    }
}
