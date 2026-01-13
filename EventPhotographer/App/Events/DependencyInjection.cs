namespace EventPhotographer.App.Events;

public static class DependencyInjection
{
    public static IServiceCollection AddEventsModule(this IServiceCollection services)
    {
        services.AddScoped<Services.EventService>();

        return services;
    }
}
