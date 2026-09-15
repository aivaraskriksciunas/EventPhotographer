using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using System.Reflection;

namespace EventPhotographer.Core.Startup;

public class RegisterMessageConsumers(
    IBus bus,
    IServiceProvider serviceProvider)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var autoSubscriber = new AutoSubscriber(bus, serviceProvider, "api");

        await autoSubscriber.SubscribeAsync(Assembly.GetExecutingAssembly().GetTypes());
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
