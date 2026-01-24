using EventPhotographer.App.Events;
using EventPhotographer.App.Users;
using EventPhotographer.Core.Exceptions;
using FluentValidation;

namespace EventPhotographer.Core;

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

    public static IServiceCollection AddAppExceptionHandlers(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<ValidationExceptionHandler>();

        return services;
    }
}
