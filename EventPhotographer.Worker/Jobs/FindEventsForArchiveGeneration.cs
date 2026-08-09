using EventPhotographer.Core;
using EventPhotographer.Core.Features.Content.Entities;
using EventPhotographer.Worker.Workers;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace EventPhotographer.Worker.Jobs;

internal class FindEventsForArchiveGeneration(
    AppDbContext dbContext,
    IBackgroundJobClient backgroundJobs)
{
    public async Task ExecuteAsync()
    {
        var cutoffDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(14));
        var endedEvent = await dbContext.Events
            .Where(e => e.EndDate <= DateTime.UtcNow)
            .Where(e => e.EndDate >= cutoffDate) // Only consider events that ended recently
            .Where(e => !e.Media.Any(m => m.Type == MediaType.Archive))
            .Where(e => e.Media.Any(m => m.Type == MediaType.UserUpload))
            .FirstOrDefaultAsync();

        if (endedEvent == null)
        {
            return;
        }

        backgroundJobs.Enqueue<CreateEventFileArchiveJob>(e => e.Execute(endedEvent.Id));
    }
}
