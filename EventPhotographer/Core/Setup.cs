using EventPhotographer.App.AccountPolicies;
using EventPhotographer.App.Content;
using EventPhotographer.App.Events;
using EventPhotographer.App.Events.Authorization.Requirements;
using EventPhotographer.App.Users;
using EventPhotographer.App.Users.Entities;
using EventPhotographer.Core.Attributes;
using EventPhotographer.Core.Configuration;
using EventPhotographer.Core.Exceptions;
using EventPhotographer.Core.Middleware;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace EventPhotographer.Core;

public static class Setup
{
    public static IServiceCollection AddAppModules(this IServiceCollection services)
    {
        services.AddEventsModule();
        services.AddUsersModule();
        services.AddAccountPoliciesModule();
        services.AddContentModule();

        // Load FluentValidation validators from this assembly
        services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Scoped);

        services.AddScoped<ParticipantMiddleware>();
        
        return services;
    }

    public static IServiceCollection AddAppExceptionHandlers(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<ValidationExceptionHandler>();

        return services;
    }

    public static IServiceCollection AddAppAuth(this IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole>(
            options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>();

        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.SlidingExpiration = true;
            options.ExpireTimeSpan = new TimeSpan(1, 0, 0); // 1 Hour
            options.Cookie.Name = "AuthenticationCookie";
        });

        services.AddAuthorizationBuilder()
            .AddPolicy(ActiveParticipantRequiredAttribute.POLICY_NAME, policy => policy.Requirements.Add(new IsActiveParticipantRequirement()));

        return services;
    }

    public static IServiceCollection AddConfiguration(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<ShareableLinkOptions>(configuration.GetSection("ShareableLink"));
        services.Configure<ObjectStorageConfiguration>(configuration.GetSection("ObjectStorage"));

        return services;
    }

    public static WebApplication UseApplicationMiddleware(this WebApplication app)
    {
        app.UseMiddleware<ParticipantMiddleware>();

        return app;
    }
}
