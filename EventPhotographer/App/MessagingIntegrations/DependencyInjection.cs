using EventPhotographer.App.MessagingIntegrations.Services.WhatsApp;

namespace EventPhotographer.App.MessagingIntegrations;

public static class DependencyInjection
{
    public static void AddMessagingIntegrationsModule(this IServiceCollection services)
    {
        services.AddScoped<WhatsAppWebhookService>();
    }
}
