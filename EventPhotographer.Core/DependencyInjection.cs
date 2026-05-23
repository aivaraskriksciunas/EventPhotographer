using EventPhotographer.Core.Features.Content;
using EventPhotographer.Core.Features.Events;
using EventPhotographer.Core.Features.MessagingIntegrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EventPhotographer.Core;

public static class DependencyInjection
{
    public static void AddDataServices(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(
            options => options.UseNpgsql(connectionString)
        );
    }

    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddEventServices();
        services.AddContentServices();
        services.AddMessagingIntegrationServices();
    }
}
