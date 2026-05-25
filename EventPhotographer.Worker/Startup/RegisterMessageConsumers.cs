using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using System.Reflection;

namespace EventPhotographer.Worker.Startup;

internal class RegisterMessageConsumers(
    IBus bus,
    IServiceProvider serviceProvider)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var autoSubscriber = new AutoSubscriber(bus, serviceProvider, "worker");

        await autoSubscriber.SubscribeAsync(Assembly.GetExecutingAssembly().GetTypes());
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
