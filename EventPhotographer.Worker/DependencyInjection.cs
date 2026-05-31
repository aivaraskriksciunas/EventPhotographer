using EventPhotographer.Worker.Configuration;
using EventPhotographer.Worker.Consumers;
using EventPhotographer.Worker.Services.MessagingIntegrations.WhatsApp;
using EventPhotographer.Worker.Services.MessagingIntegrations.WhatsApp.MessageContentProcessors;
using System.Net.Http.Headers;

namespace EventPhotographer.Worker;

internal static class DependencyInjection
{
    public static void AddWorkerConsumers(this IServiceCollection services)
    {
        services.AddScoped<CreateCompressedEventFileArchiveConsumer>();
        services.AddScoped<ProcessWhatsAppWebhookPayloadConsumer>();
        services.AddScoped<CreateWhatsAppMessageLinkConsumer>();
    }

    public static void AddWorkerServices(this IServiceCollection services)
    {
        services.AddScoped<WhatsAppWebhookPayloadProcessor>();

        services.AddScoped<MessageContentProcessorFactory>();
        services.AddKeyedTransient<IMessageContentProcessor, TextMessageProcessor>(TextMessageProcessor.MessageType);
        services.AddKeyedTransient<IMessageContentProcessor, VideoMessageProcessor>(VideoMessageProcessor.MessageType);
        services.AddKeyedTransient<IMessageContentProcessor, ImageMessageProcessor>(ImageMessageProcessor.MessageType);
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
