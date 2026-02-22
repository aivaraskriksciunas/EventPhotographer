using EventPhotographer.App.Events.Authorization.Handlers;
using Microsoft.AspNetCore.Authorization;

namespace EventPhotographer.App.Events;

public static class DependencyInjection
{
    public static IServiceCollection AddEventsModule(this IServiceCollection services)
    {
        services.AddScoped<Services.EventService>();
        services.AddScoped<Services.EventShareableLinkService>();

        // Authorization
        services.AddSingleton<IAuthorizationHandler, EventAccessRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, CreateShareableLinkRequirementHandler>();

        return services;
    }
}
