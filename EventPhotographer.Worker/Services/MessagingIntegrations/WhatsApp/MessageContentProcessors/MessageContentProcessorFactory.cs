using EventPhotographer.Core.Features.MessagingIntegrations.Entities;

namespace EventPhotographer.Worker.Services.MessagingIntegrations.WhatsApp.MessageContentProcessors;

internal class MessageContentProcessorFactory(IServiceProvider serviceProvider)
{
    public IMessageContentProcessor? MakeProcessor(WhatsAppMessage message)
    {
        return serviceProvider.GetKeyedService<IMessageContentProcessor>(message.Type);
    }
}
