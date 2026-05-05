using EventPhotographer.Core.Features.Content.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EventPhotographer.Core.Features.Content;

internal static class DependencyInjection
{
    public static void AddContentServices(this IServiceCollection services)
    {
        services.AddSingleton<FileContentTypeReader>();
        services.AddScoped<MediaService>();
        services.AddScoped<MediaStorageService>();
    }
}
