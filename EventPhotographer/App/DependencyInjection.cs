using EventPhotographer.App.Events;
using EventPhotographer.App.Users;
using FluentValidation;

namespace EventPhotographer.App;

public static class DependencyInjection
{
    public static IServiceCollection AddAppModules(this IServiceCollection services)
    {
        services.AddEventsModule();
        services.AddUsersModule();

        // Load FluentValidation validators from this assembly
        services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Scoped);
        
        return services;
    }
}
