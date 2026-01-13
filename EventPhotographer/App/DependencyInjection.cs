using EventPhotographer.App.Events;

namespace EventPhotographer.App;

public static class DependencyInjection
{
    public static IServiceCollection AddAppModules(this IServiceCollection services)
    {
        services.AddEventsModule();
        
        return services;
    }
}
