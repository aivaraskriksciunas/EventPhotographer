using EventPhotographer.App.Events.Services;
using EventPhotographer.Core.Features.Events.Entities;
using EventPhotographer.Core.Features.Events.Services;
using EventPhotographer.Core.Features.MessagingIntegrations.Entities;
using EventPhotographer.Core.Features.MessagingIntegrations.Services;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace EventPhotographer.Worker.Services.MessagingIntegrations.WhatsApp.MessageContentProcessors;

internal class TextMessageProcessor(
    WhatsAppTextService whatsAppTextService,
    WhatsAppContactService whatsAppContactService,
    WhatsAppClient whatsAppClient,
    EventShareableLinkService shareableLinkService,
    ParticipantService participantService,
    EventPermissionsService eventPermissionsService) 
    : IMessageContentProcessor
{
    public static string MessageType => "text";

    public async Task ProcessMessageContentAsync(WhatsAppMessage message, JsonElement json)
    {
        var body = json.GetProperty("text").GetProperty("body").GetString();
        
        await whatsAppTextService.CreateAsync(message, body ?? "");
        await whatsAppClient.MarkAsReadAsync(message);

        // Interpret message
        var match = Regex.Match(body ?? "", @"\b\d{6}\b");
        if (!match.Success)
        {
            await ReplyWithCurrentEvent(message);
            return;
        }

        string code = match.Value;
        var shareableLink = await shareableLinkService.GetShareableLinkByCode(code);
        if (shareableLink?.Event == null || !eventPermissionsService.CanJoinEvent(shareableLink.Event))
        {
            await whatsAppClient.ReplyToMessage(message, "I don't know any event with this code :(");
            return;
        }

        // Join event and send confirmation message
        var participant = await participantService.CreateOrGetParticipantAsync(shareableLink, message.WhatsAppContact.ProfileName);
        message.WhatsAppContact.ActiveParticipant = participant;
        await whatsAppContactService.UpdateAsync(message.WhatsAppContact);

        await whatsAppClient.ReplyToMessage(
            message,
            $"You have joined '{participant.Event.Name}'! You can send me the pictures you want to share with the hosts at any time. I will react to them when I receive them :) Have fun!");
    }

    private async Task ReplyWithCurrentEvent(WhatsAppMessage message)
    {
        Participant? participant = null;
        if (message.WhatsAppContact.ActiveParticipantId != null)
        {
            participant = await participantService.GetById((Guid)message.WhatsAppContact.ActiveParticipantId);
        }

        if (participant != null)
        {
            await whatsAppClient.ReplyToMessage(message, $"You are currently sending pictures to '{participant.Event.Name}'. Send me the code of another event to switch, or create your own event at livealbum.eu.");
            return;
        }

        await whatsAppClient.ReplyToMessage(message, "Please enter the code of an ongoing event, or create your own at livealbum.eu.");
    }
}
