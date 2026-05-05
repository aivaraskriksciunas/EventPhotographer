using EventPhotographer.App.Events.Services;
using EventPhotographer.Core.Features.Events.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EventPhotographer.Core.Features.Events;

internal static class DependencyInjection
{
    public static void AddEventServices(this IServiceCollection services)
    {
        services.AddScoped<EventService>();
        services.AddScoped<EventShareableLinkService>();
        services.AddScoped<ParticipantService>();
    }
}
