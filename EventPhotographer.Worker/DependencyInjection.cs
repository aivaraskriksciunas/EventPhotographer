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
    }

    public static void AddWorkerServices(this IServiceCollection services)
    {
        services.AddScoped<WhatsAppWebhookPayloadProcessor>();

        services.AddScoped<IMessageContentProcessor, MediaMessageProcessor>();
        services.AddScoped<IMessageContentProcessor, TextMessageProcessor>();
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
    }
}
