using EventPhotographer.Worker.Consumers;

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
        services.AddScoped<Services.MessagingIntegrations.WhatsApp.WhatsAppWebhookPayloadProcessor>();
    }
}
