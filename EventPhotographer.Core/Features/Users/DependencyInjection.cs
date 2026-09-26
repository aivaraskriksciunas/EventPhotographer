using EventPhotographer.Core.Features.Users.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EventPhotographer.Core.Features.Users;

internal static class DependencyInjection
{
    public static void AddUserServices(this IServiceCollection services)
    {
        services.AddScoped<AccountVerificationService>();
    }
}
