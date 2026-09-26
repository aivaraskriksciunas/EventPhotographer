using EventPhotographer.Core;
using EventPhotographer.Core.Configuration;
using EventPhotographer.Core.Startup;
using EventPhotographer.UseCases;
using EventPhotographer.Worker;
using EventPhotographer.Worker.Configuration;
using EventPhotographer.Worker.Startup;
using Sentry.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<SentryLoggingOptions>(builder.Configuration.GetSection("Sentry"));
builder.Services.Configure<WhatsAppConfiguration>(builder.Configuration.GetSection("WhatsApp"));
builder.Services.Configure<ObjectStorageConfiguration>(builder.Configuration.GetSection("ObjectStorage"));
builder.Services.Configure<SmtpConfiguration>(builder.Configuration.GetSection("Smtp"));

builder.Logging.AddSentry();

// Database
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new ApplicationException("Database DefaultConnection is not provided");
builder.Services.AddDataServices(connectionString);

// Object Storage
builder.Services.AddObjectStorage(
    builder.Configuration.GetSection("ObjectStorage").Get<ObjectStorageConfiguration>() ?? throw new ApplicationException("ObjectStorage configuration is not provided"));

// RabbitMQ + EasyNetQ
builder.Services.AddApplicationMessageQueues(
    builder.Configuration.GetConnectionString("RabbitMq") ?? throw new ApplicationException("RabbitMq connection string is not provided"));
builder.Services.AddHostedService<RegisterMessageConsumers>();

// Services
builder.Services.AddWorkerHttpClients(builder.Configuration);
builder.Services.AddUseCases();
builder.Services.AddApplicationServices();
builder.Services.AddWorkerConsumers();
builder.Services.AddWorkerServices();
builder.Services.AddScheduler(connectionString);
builder.Services.AddHostedService<ScheduleRecurringJobs>();

var host = builder.Build();
host.Run();
