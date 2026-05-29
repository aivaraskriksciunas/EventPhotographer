using EventPhotographer.App.Events.Services;
using EventPhotographer.Core.Features.Content.Services;
using EventPhotographer.Core.Features.Events.Services;
using EventPhotographer.Core.Features.MessagingIntegrations.Entities;
using EventPhotographer.Core.Features.MessagingIntegrations.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace EventPhotographer.Worker.Services.MessagingIntegrations.WhatsApp.MessageContentProcessors;

internal sealed class ImageMessageProcessor : BaseMediaMessageProcessor
{
    public ImageMessageProcessor(
        WhatsAppMediaService whatsAppMediaService, 
        WhatsAppClient whatsAppClient, 
        EventPermissionsService eventPermissionsService,
        ParticipantService participantService,
        MediaService mediaService) 
        : base(whatsAppMediaService, whatsAppClient, eventPermissionsService, participantService, mediaService)
    {}

    public static string MessageType => "image";

    public override async Task ProcessMessageContentAsync(WhatsAppMessage message, JsonElement json)
    {
        var mediaJson = json.GetProperty("image");

        await ProcessMediaAsync(message, mediaJson);
    }
}
