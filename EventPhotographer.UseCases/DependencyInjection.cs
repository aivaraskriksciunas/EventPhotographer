using EventPhotographer.UseCases.Common;
using EventPhotographer.UseCases.Common.Authorization;
using EventPhotographer.UseCases.Common.CommandFilters;
using EventPhotographer.UseCases.Common.Commands;
using EventPhotographer.UseCases.Common.Decorators;
using EventPhotographer.UseCases.Common.PipelineBehaviours;
using EventPhotographer.UseCases.Events;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EventPhotographer.UseCases;

public static class DependencyInjection
{
    public static void AddUseCases(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblyOf<Result>()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime())  
        ;

        // Register command filters. Authorization must go first for security
        services.AddScoped<ICommandFilter, AuthorizationFilter>();
        services.AddScoped<ICommandFilter, ValidationFilter>();
        services.Decorate(typeof(ICommandHandler<,>), typeof(FilterDecorator<,>));
        
        // Register authorization handlers
        services.AddScoped<AuthorizationService>();
        services.Scan(scan => scan 
            .FromAssemblyOf<AuthorizationService>()
            .AddClasses(classes => classes.AssignableTo(typeof(IAuthorizationHandler<>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );

        // Register queries
        services.AddScoped<EventQueryService>();

        services.AddValidatorsFromAssemblyContaining<AuthorizationService>(ServiceLifetime.Scoped, includeInternalTypes: true);
    }
}
