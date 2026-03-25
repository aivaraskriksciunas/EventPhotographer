using EventPhotographer.App.Content.Authorization;
using EventPhotographer.App.Content.Services;
using Microsoft.AspNetCore.Authorization;

namespace EventPhotographer.App.Content;

public static class DependencyInjection
{
    public static IServiceCollection AddContentModule(this IServiceCollection services)
    {
        services.AddScoped<MediaService>();

        services.AddScoped<IAuthorizationHandler, ManageMediaRequirementHandler>();

        return services;
    }
}
