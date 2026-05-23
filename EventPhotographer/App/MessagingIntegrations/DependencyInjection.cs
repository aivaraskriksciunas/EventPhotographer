using EventPhotographer.App.MessagingIntegrations.Services;
using System.ComponentModel.Design;

namespace EventPhotographer.App.MessagingIntegrations;

public static class DependencyInjection
{
    public static void AddMessagingIntegrationsModule(this IServiceCollection services)
    {
        services.AddScoped<WhatsAppWebhookService>();
    }
}
