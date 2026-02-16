using EventPhotographer.App.AccountPolicies.Services;

namespace EventPhotographer.App.AccountPolicies;

public static class DependencyInjection
{
    public static IServiceCollection AddAccountPoliciesModule(this IServiceCollection services)
    {
        services.AddScoped<AccountTierService>();

        return services;
    }
}
