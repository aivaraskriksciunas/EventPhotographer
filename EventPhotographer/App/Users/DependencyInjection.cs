using EventPhotographer.App.Users.Services;

namespace EventPhotographer.App.Users;
public static class DependencyInjection
{
    public static IServiceCollection AddUsersModule(this IServiceCollection services)
    {
        services.AddScoped<AccountService>();

        return services;
    }
}