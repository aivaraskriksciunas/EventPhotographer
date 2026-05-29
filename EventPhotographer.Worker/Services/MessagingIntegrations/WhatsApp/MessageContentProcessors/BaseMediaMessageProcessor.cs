using EventPhotographer.App.Events.Services;
using EventPhotographer.Core.Features.Content.Services;
using EventPhotographer.Core.Features.Events.Entities;
using EventPhotographer.Core.Features.Events.Services;
using EventPhotographer.Core.Features.MessagingIntegrations.Entities;
using EventPhotographer.Core.Features.MessagingIntegrations.Services;
using System.Security.Cryptography;
using System.Text.Json;

namespace EventPhotographer.Worker.Services.MessagingIntegrations.WhatsApp.MessageContentProcessors;

abstract internal class BaseMediaMessageProcessor(
    WhatsAppMediaService whatsAppMediaService,
    WhatsAppClient whatsAppClient,
    WhatsAppMediaClient whatsAppMediaClient,
    EventPermissionsService eventPermissionsService,
    ParticipantService participantService,
    MediaService mediaService) 
    : IMessageContentProcessor
{
    public abstract Task ProcessMessageContentAsync(WhatsAppMessage message, JsonElement json);

    protected async Task ProcessMediaAsync(WhatsAppMessage message, JsonElement mediaJson)
    {
        // Log media
        var media = new WhatsAppMedia
        {
            Caption = DateTime.Now.ToString("yyyyMMdd_HHmmss"),
            MimeType = mediaJson.GetProperty("mime_type").GetString() ?? throw new KeyNotFoundException(),
            WhatsAppId = mediaJson.GetProperty("id").GetString() ?? throw new KeyNotFoundException(),
            Url = mediaJson.GetProperty("url").GetString(),
            WhatsAppMessage = message,
        };

        await whatsAppMediaService.AddAsync(media);
        await whatsAppClient.MarkAsReadAsync(message);

        if (!FileContentTypeReader.IsAllowedMimeType(media.MimeType))
        {
            await whatsAppClient.ReplyToMessage(message, "Sorry, but I do not support this type of files. Please send me only images or videos.");
            return;
        }

        // Check participation
        Participant? participant = null;
        if (message.WhatsAppContact.ActiveParticipantId != null)
        {
            participant = await participantService.GetById((Guid)message.WhatsAppContact.ActiveParticipantId);
        }

        if (participant == null)
        {
            await whatsAppClient.ReplyToMessage(message, "I don't know what event you're part of! Please text me the event code first before sending any pictures.");
            return;
        }

        if (!eventPermissionsService.CanUploadEventMedia(participant.Event))
        {
            await whatsAppClient.ReplyToMessage(message, $"Sadly '{participant.Event.Name}' does not accept pictures any more. Enter the code of the new event or create your own at livealbum.eu!");
            return;
        }

        var mediaEntity = await mediaService.CreateMedia(participant);

        await mediaService.UploadFile(
            mediaEntity,
            await whatsAppMediaClient.DownloadMediaAsync(media),
            FileContentTypeReader.GetExtensionFromMimeType(media.MimeType)!);

        await whatsAppClient.ReactToMessage(message, "\u2764\uFE0F");
        await ReplyWithCompliment(message);
    }

    public async Task ReplyWithCompliment(WhatsAppMessage message)
    {
        // 5% chance of replying with a compliment
        var chance = RandomNumberGenerator.GetInt32(100);
        if (chance >= 5) return;

        string[] compliments = 
        {
            "Wow! 🔥",
            "Thanks for sharing! The hosts will be amazed",
            "Looking great!",
            "You're a star! ⭐",
            "Here's a star for this awesome picture: ⭐",
            "That's amazing",
            "Chef's kiss! 😘",
            "This is art!",
        };

        var chosenCompliment = compliments[Random.Shared.Next(compliments.Length)];
        await whatsAppClient.ReplyToMessage(message, chosenCompliment);
    }
}
