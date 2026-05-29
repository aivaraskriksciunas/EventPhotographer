using EventPhotographer.App.Events.Services;
using EventPhotographer.Core.Features.Content.Services;
using EventPhotographer.Core.Features.Events.Services;
using EventPhotographer.Core.Features.MessagingIntegrations.Entities;
using EventPhotographer.Core.Features.MessagingIntegrations.Services;
using System.Text.Json;

namespace EventPhotographer.Worker.Services.MessagingIntegrations.WhatsApp.MessageContentProcessors;

internal class VideoMessageProcessor : BaseMediaMessageProcessor
{
    public VideoMessageProcessor(
        WhatsAppMediaService whatsAppMediaService, 
        WhatsAppClient whatsAppClient, 
        WhatsAppMediaClient whatsAppMediaClient,
        EventPermissionsService eventPermissionsService,
        ParticipantService participantService,
        MediaService mediaService) 
        : base(whatsAppMediaService, whatsAppClient, whatsAppMediaClient, eventPermissionsService, participantService, mediaService)
    {}

    public static string MessageType => "video";

    public override async Task ProcessMessageContentAsync(WhatsAppMessage message, JsonElement json)
    {
        var mediaJson = json.GetProperty("video");

        await ProcessMediaAsync(message, mediaJson);
    }
}
