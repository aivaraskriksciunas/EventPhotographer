using Hangfire;
using Hangfire.PostgreSql;

namespace EventPhotographer.Worker.Startup;

internal static class SchedulerSetup
{
    public static void AddScheduler(this IServiceCollection services, string connectionString)
    {
        services.AddHangfire(options => options
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(connectionString))
        );

        services.AddHangfireServer();
    }
}
