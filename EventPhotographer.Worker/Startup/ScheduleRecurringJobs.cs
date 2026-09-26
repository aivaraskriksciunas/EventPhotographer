using EventPhotographer.Worker.Jobs;
using Hangfire;

namespace EventPhotographer.Worker.Startup;

internal class ScheduleRecurringJobs(
    IRecurringJobManager backgroundJobs) 
    : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        backgroundJobs.AddOrUpdate<FindEventsForArchiveGeneration>(
            "find-events-for-archive-generator",
            m => m.ExecuteAsync(), 
            Cron.MinuteInterval(15));

        backgroundJobs.AddOrUpdate<SendScheduledEmailsJob>(
            "send-scheduled-emails",
            m => m.ExecuteAsync(),
            Cron.Minutely());

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
