using EventPhotographer.Worker.Configuration;
using EventPhotographer.Worker.Consumers;
using EventPhotographer.Worker.Services.Emails;
using EventPhotographer.Worker.Services.Emails.Providers;
using EventPhotographer.Worker.Services.MessagingIntegrations.WhatsApp;
using EventPhotographer.Worker.Services.MessagingIntegrations.WhatsApp.MessageContentProcessors;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace EventPhotographer.Worker;

internal static class DependencyInjection
{
    public static void AddWorkerConsumers(this IServiceCollection services)
    {
        services.AddScoped<CreateCompressedEventFileArchiveConsumer>();
        services.AddScoped<ProcessWhatsAppWebhookPayloadConsumer>();
        services.AddScoped<ValidateUploadedFileMessageConsumer>();
        services.AddScoped<SendEmailMessageConsumer>();
    }

    public static void AddWorkerServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<WhatsAppWebhookPayloadProcessor>();

        services.AddScoped<MessageContentProcessorFactory>();
        services.AddKeyedTransient<IMessageContentProcessor, TextMessageProcessor>(TextMessageProcessor.MessageType);
        services.AddKeyedTransient<IMessageContentProcessor, VideoMessageProcessor>(VideoMessageProcessor.MessageType);
        services.AddKeyedTransient<IMessageContentProcessor, ImageMessageProcessor>(ImageMessageProcessor.MessageType);

        // Inject Email sender
        var emailProvider = configuration["Email:Provider"];
        switch (emailProvider)
        {
            case "Hostinger":
                var config = configuration.GetSection("HostingerEmail");
                services.AddOptions<HostingerEmailConfiguration>()
                    .Bind(configuration.GetSection("HostingerEmail"))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

                services.AddHttpClient<IEmailSender, HostingerEmailProvider>((sp, client) =>
                {
                    var hostingerConfig = sp.GetRequiredService<IOptions<HostingerEmailConfiguration>>().Value;

                    client.BaseAddress = new Uri("https://api.mail.hostinger.com/api/v1/");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", hostingerConfig.Token);
                });
                break;
            case "Smtp":
                services.AddScoped<IEmailSender, SmtpEmailProvider>();
                break;
            default:
                throw new InvalidOperationException($"Unknown email provider: '{emailProvider}'. Expected 'Smtp' or 'Hostinger'.");
        }
    }

    public static void AddWorkerHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        var whatsAppConfig = configuration.GetSection("WhatsApp").Get<WhatsAppConfiguration>() 
            ?? throw new ArgumentNullException("WhatsApp configuration not provided");

        services.AddHttpClient<WhatsAppClient>(client =>
        {
            client.BaseAddress = new Uri($"https://graph.facebook.com/{whatsAppConfig.ApiVersion}/{whatsAppConfig.BusinessPhoneNumberId}/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", whatsAppConfig.AccessToken);
        });
        services.AddHttpClient<WhatsAppMediaClient>(client =>
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", whatsAppConfig.AccessToken);
        });
    }
}
