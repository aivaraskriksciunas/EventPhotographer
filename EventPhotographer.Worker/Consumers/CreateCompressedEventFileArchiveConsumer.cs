using EasyNetQ.AutoSubscribe;
using EventPhotographer.Core;
using EventPhotographer.Core.Features.Content.Services;
using EventPhotographer.Core.Features.Events.Messages;

namespace EventPhotographer.Worker.Consumers;

internal class CreateCompressedEventFileArchiveConsumer()
    : IConsumeAsync<CreateCompressedEventFileArchive>
{

    public Task ConsumeAsync(CreateCompressedEventFileArchive message, CancellationToken cancellationToken = default)
    {

        return Task.CompletedTask;
    }

}
