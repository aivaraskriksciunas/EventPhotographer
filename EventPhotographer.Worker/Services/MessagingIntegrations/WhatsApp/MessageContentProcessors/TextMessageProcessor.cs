using EventPhotographer.Core.Features.MessagingIntegrations.Entities;
using EventPhotographer.Core.Features.MessagingIntegrations.Services;
using System.Text.Json;

namespace EventPhotographer.Worker.Services.MessagingIntegrations.WhatsApp.MessageContentProcessors;

internal class TextMessageProcessor(
    WhatsAppTextService whatsAppTextService,
    WhatsAppClient whatsAppClient) 
    : IMessageContentProcessor
{
    public async Task ProcessMessageContentAsync(WhatsAppMessage message, JsonElement json)
    {
        var body = json.GetProperty("text").GetProperty("body").GetString();
        
        // Register message in the log
        await whatsAppTextService.CreateAsync(message, body ?? "");

        // Send reply
        await whatsAppClient.MarkAsReadAsync(message);
        await whatsAppClient.ReplyToMessage(message, "Hello!");
    }

    public bool Supports(WhatsAppMessage message)
    {
        return message.Type == "text";
    }
}
