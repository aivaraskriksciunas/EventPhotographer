using EventPhotographer.Worker.Workers;
using Quartz;

namespace EventPhotographer.Worker.Startup;

internal static class SchedulerSetup
{
    public static void AddScheduler(this IServiceCollection services)
    {
        services.AddQuartz(options =>
        {
            var fileCompressorJob = JobKey.Create(nameof(EventCompressedFileGenerator));
            options
                .AddJob<EventCompressedFileGenerator>(fileCompressorJob)
                .AddTrigger(trigger =>
                {
                    trigger
                        .ForJob(fileCompressorJob)
                        .WithSimpleSchedule(action => action.WithIntervalInSeconds(60).RepeatForever());
                });
        });
    }
}
