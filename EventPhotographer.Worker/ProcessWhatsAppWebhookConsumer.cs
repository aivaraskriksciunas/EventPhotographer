using EasyNetQ.AutoSubscribe;
using EventPhotographer.Core;
using EventPhotographer.Core.Features.MessagingIntegrations.Messages;
using EventPhotographer.Core.Features.MessagingIntegrations.Services;
using EventPhotographer.Worker.Services.MessagingIntegrations.WhatsApp;
using System.Text;
using System.Text.Json;

namespace EventPhotographer.Worker.Consumers;

internal class ProcessWhatsAppWebhookPayloadConsumer(
    AppDbContext db,
    WhatsAppWebhookPayloadService payloadService,
    WhatsAppWebhookPayloadProcessor payloadProcessor)
    : IConsumeAsync<ProcessWhatsAppWebhookPayload>
{
    public async Task ConsumeAsync(
        ProcessWhatsAppWebhookPayload message, 
        CancellationToken cancellationToken = default)
    {
        var payload = await payloadService.GetAsync(message.WhatsAppPayloadId);
        if (payload is null
            || !payload.IsValid
            || payload.IsProcessed) 
        {
            return;
        }

        payload.IsProcessed = true;
        await db.SaveChangesAsync();

        var json = JsonDocument.Parse(payload.Payload);
        await payloadProcessor.HandlePayload(json);
    }
}
