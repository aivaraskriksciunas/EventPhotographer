using EventPhotographer.App.Events;
using FluentValidation;

namespace EventPhotographer.App;

public static class DependencyInjection
{
    public static IServiceCollection AddAppModules(this IServiceCollection services)
    {
        services.AddEventsModule();

        // Load FluentValidation validators from this assembly
        services.AddValidatorsFromAssemblyContaining<Program>();
        
        return services;
    }
}
