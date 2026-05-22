using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;

namespace EventPhotographer.Core.Startup;

public static class MessageQueuesSetup
{
    public static void AddApplicationMessageQueues(
        this IServiceCollection services, 
        string connectionString)
    {
        services.AddEasyNetQ(connectionString)
            .UseSystemTextJsonV2();
    }
}
