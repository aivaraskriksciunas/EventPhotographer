using EventPhotographer.Core.Features.MessagingIntegrations.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EventPhotographer.Core.Features.MessagingIntegrations;

public static class DependencyInjection
{
    public static void AddMessagingIntegrationServices(this IServiceCollection services)
    {
        services.AddScoped<WhatsAppIntegrationService>();
        services.AddScoped<WhatsAppWebhookPayloadService>();
        services.AddScoped<WhatsAppMessageService>();
        services.AddScoped<WhatsAppContactService>();
        services.AddScoped<WhatsAppTextService>();
        services.AddScoped<WhatsAppMediaService>();
        services.AddScoped<WhatsAppMessageLinkService>();
    }
}
