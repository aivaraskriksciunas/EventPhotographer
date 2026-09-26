using EventPhotographer.Core.Features.Emails.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EventPhotographer.Core.Features.Emails;

internal static class DependencyInjection
{
    public static void AddEmailServices(this IServiceCollection services)
    {
        services.AddScoped<TemplateEmailService>();
    }
}
