using EasyNetQ.Producer;
using EventPhotographer.Core.Features.MessagingIntegrations.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventPhotographer.Core.Features.MessagingIntegrations;

public static class DependencyInjection
{
    public static void AddMessagingIntegrationServices(this IServiceCollection services)
    {
        services.AddScoped<WhatsappIntegrationService>();
    }
}
