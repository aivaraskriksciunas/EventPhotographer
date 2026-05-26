using EventPhotographer.Core.Features.MessagingIntegrations.Entities;
using EventPhotographer.Core.Features.MessagingIntegrations.Services;
using System.Text.Json;

namespace EventPhotographer.Worker.Services.MessagingIntegrations.WhatsApp.MessageContentProcessors;

internal class MediaMessageProcessor(
    WhatsAppMediaService whatsAppMediaService) : IMessageContentProcessor
{
    public async Task ProcessMessageContentAsync(WhatsAppMessage message, JsonElement json)
    {
        var mediaJson = json.GetProperty("video");

        var media = new WhatsAppMedia
        {
            Caption = mediaJson.GetProperty("caption").GetString(),
            MimeType = mediaJson.GetProperty("mime_type").GetString() ?? throw new KeyNotFoundException(),
            WhatsAppId = mediaJson.GetProperty("id").GetString() ?? throw new KeyNotFoundException(),
            Url = mediaJson.GetProperty("url").GetString(),
            WhatsAppMessage = message,
        };

        await whatsAppMediaService.AddAsync(media);
    }

    public bool Supports(WhatsAppMessage message)
    {
        return message.Type == "image" || message.Type == "video";
    }
}
