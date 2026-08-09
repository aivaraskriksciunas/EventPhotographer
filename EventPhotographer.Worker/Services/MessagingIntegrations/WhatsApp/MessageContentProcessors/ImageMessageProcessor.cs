using EventPhotographer.App.Events.Services;
using EventPhotographer.Core.Features.Content.Entities;
using EventPhotographer.Core.Features.Content.Services;
using EventPhotographer.Core.Features.Events.Services;
using EventPhotographer.Core.Features.MessagingIntegrations.Entities;
using EventPhotographer.Core.Features.MessagingIntegrations.Services;
using EventPhotographer.UseCases.Common.Authorization;
using EventPhotographer.UseCases.Common.Commands;
using EventPhotographer.UseCases.Content.Commands;
using System.Text.Json;

namespace EventPhotographer.Worker.Services.MessagingIntegrations.WhatsApp.MessageContentProcessors;

internal sealed class ImageMessageProcessor : BaseMediaMessageProcessor
{
    public ImageMessageProcessor(
        WhatsAppMediaService whatsAppMediaService, 
        WhatsAppClient whatsAppClient, 
        WhatsAppMediaClient whatsAppMediaClient,
        AuthorizationService authorizationService,
        ParticipantService participantService,
        ICommandHandler<UploadFileCommand, MediaFile> uploadFileHandler) 
        : base(whatsAppMediaService, whatsAppClient, whatsAppMediaClient, authorizationService, participantService, uploadFileHandler)
    {}

    public static string MessageType => "image";

    public override async Task ProcessMessageContentAsync(WhatsAppMessage message, JsonElement json)
    {
        var mediaJson = json.GetProperty("image");

        await ProcessMediaAsync(message, mediaJson);
    }
}
