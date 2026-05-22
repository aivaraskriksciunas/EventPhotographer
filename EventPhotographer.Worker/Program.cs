using EventPhotographer.Core.Configuration;
using EventPhotographer.Core.Startup;
using EventPhotographer.Core;
using Quartz;
using EventPhotographer.Worker.Startup;
using Sentry.Extensions.Logging;
using EventPhotographer.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<SentryLoggingOptions>(builder.Configuration.GetSection("Sentry"));

builder.Logging.AddSentry();

// Database
builder.Services.AddDataServices(
    builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new ApplicationException("Database DefaultConnection is not provided"));

// Object Storage
builder.Services.AddObjectStorage(
    builder.Configuration.GetSection("ObjectStorage").Get<ObjectStorageConfiguration>() ?? throw new ApplicationException("ObjectStorage configuration is not provided"));

// RabbitMQ + EasyNetQ
builder.Services.AddApplicationMessageQueues(
    builder.Configuration.GetConnectionString("RabbitMq") ?? throw new ApplicationException("RabbitMq connection string is not provided"));
builder.Services.AddHostedService<RegisterMessageConsumers>();

// Servicess
builder.Services.AddApplicationServices();
builder.Services.AddWorkerServices();
builder.Services.AddScheduler();

builder.Services.AddQuartzHostedService(opt =>
{
    opt.WaitForJobsToComplete = true;
});

var host = builder.Build();
host.Run();
