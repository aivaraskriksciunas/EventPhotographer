using EasyNetQ.AutoSubscribe;
using EventPhotographer.Core.Features.MessagingIntegrations.Messages;
using EventPhotographer.Core.Features.MessagingIntegrations.Services;
using EventPhotographer.Worker.Services.MessagingIntegrations.WhatsApp;

namespace EventPhotographer.Worker.Consumers;

internal class CreateWhatsAppMessageLinkConsumer(
    WhatsAppMessageLinkService messageLinkService,
    WhatsAppClient whatsAppClient) 
    : IConsumeAsync<CreateWhatsAppMessageLink>
{
    public async Task ConsumeAsync(CreateWhatsAppMessageLink message, CancellationToken cancellationToken = default)
    {
        var link = await messageLinkService.GetByIdAsync(message.WhatsAppMessageLinkId);
        if (link == null) return;

        try
        {
            string body = $"Hello, I am joining {link.ShareableLink.Code}";
            var response = await whatsAppClient.CreateMessageLink(body);
            if (response == null) throw new Exception("WhatsAppClient returned an empty response");

            link.Status = Core.Features.MessagingIntegrations.Enums.WhatsAppMessageLinkStatus.Created;
            link.Code = response.Code;
            link.PrefilledMessage = body;
            link.DeepLinkUrl = response.DeepLinkUrl;

            await messageLinkService.UpdateAsync(link);
        }
        catch (Exception)
        {
            link.Status = Core.Features.MessagingIntegrations.Enums.WhatsAppMessageLinkStatus.Failed;
            await messageLinkService.UpdateAsync(link);

            throw;
        }
    }
}
