using EventPhotographer.Core;
using EventPhotographer.Core.Configuration;
using EventPhotographer.Core.Startup;
using EventPhotographer.UseCases;
using EventPhotographer.Worker;
using EventPhotographer.Worker.Configuration;
using EventPhotographer.Worker.Startup;
using Quartz;
using Sentry.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<SentryLoggingOptions>(builder.Configuration.GetSection("Sentry"));
builder.Services.Configure<WhatsAppConfiguration>(builder.Configuration.GetSection("WhatsApp"));
builder.Services.Configure<ObjectStorageConfiguration>(builder.Configuration.GetSection("ObjectStorage"));

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
builder.Services.AddWorkerHttpClients(builder.Configuration);
builder.Services.AddUseCases();
builder.Services.AddApplicationServices();
builder.Services.AddWorkerConsumers();
builder.Services.AddWorkerServices();
builder.Services.AddScheduler();

builder.Services.AddQuartzHostedService(opt =>
{
    opt.WaitForJobsToComplete = true;
});

var host = builder.Build();
host.Run();
