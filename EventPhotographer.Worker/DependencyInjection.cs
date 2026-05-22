using EventPhotographer.Worker.Consumers;

namespace EventPhotographer.Worker;

internal static class DependencyInjection
{
    public static void AddWorkerServices(this IServiceCollection services)
    {
        services.AddScoped<CreateCompressedEventFileArchiveConsumer>();
    }
}
