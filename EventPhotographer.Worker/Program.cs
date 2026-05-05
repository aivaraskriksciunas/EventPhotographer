using EventPhotographer.Core.Configuration;
using EventPhotographer.Core.Startup;
using EventPhotographer.Core;
using EventPhotographer.Worker.Workers;
using Quartz;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDataServices(
    builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new ApplicationException("Database DefaultConnection is not provided"));

builder.Services.AddObjectStorage(
    builder.Configuration.GetSection("ObjectStorage").Get<ObjectStorageConfiguration>() ?? throw new ApplicationException("ObjectStorage configuration is not provided"));

builder.Services.AddApplicationServices();

builder.Services.AddQuartz(options =>
{
    var fileCompressorJob = JobKey.Create(nameof(EventCompressedFileGenerator));
    options
        .AddJob<EventCompressedFileGenerator>(fileCompressorJob)
        .AddTrigger(trigger =>
        {
            trigger
                .ForJob(fileCompressorJob)
                .WithSimpleSchedule(action => action.WithIntervalInSeconds(10).RepeatForever());
        });
});

builder.Services.AddQuartzHostedService(opt =>
{
    opt.WaitForJobsToComplete = true;
});

var host = builder.Build();
host.Run();
