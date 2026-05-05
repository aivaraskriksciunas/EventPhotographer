using EventPhotographer.App.Content.Authorization;
using EventPhotographer.App.Content.Services;
using EventPhotographer.Core.Features.Content.Services;
using Microsoft.AspNetCore.Authorization;

namespace EventPhotographer.App.Content;

public static class DependencyInjection
{
    public static IServiceCollection AddContentModule(this IServiceCollection services)
    {
        services.AddScoped<ApiMediaService>();

        services.AddScoped<IAuthorizationHandler, ManageMediaRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, UploadFileRequirementHandler>();

        return services;
    }
}
