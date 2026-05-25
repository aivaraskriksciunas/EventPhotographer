namespace EventPhotographer.Worker.Services;

internal static class DependencyInjection
{
    public static void AddWorkerServices(this IServiceCollection services)
    {
        services.AddScoped<MessagingIntegrations.WhatsApp.WhatsAppWebhookPayloadProcessor>();
    }
}
