using EventPhotographer.Core.Features.MessagingIntegrations.Entities;
using System.Text.Json;

namespace EventPhotographer.Worker.Services.MessagingIntegrations.WhatsApp.MessageContentProcessors;

internal interface IMessageContentProcessor
{
    public Task ProcessMessageContentAsync(WhatsAppMessage message, JsonElement json);

    public bool Supports(WhatsAppMessage message);
}
