using EventPhotographer.Core.Features.MessagingIntegrations.Entities;
using EventPhotographer.Worker.Services.MessagingIntegrations.DTO;
using System.Net.Http.Json;

namespace EventPhotographer.Worker.Services.MessagingIntegrations.WhatsApp;

internal class WhatsAppClient(HttpClient httpClient)
{
    public async Task MarkAsReadAsync(WhatsAppMessage message)
    {
        var payload = new
        {
            messaging_product = "whatsapp",
            status = "read",
            message_id = message.WhatsAppId,
        };

        using var response = await httpClient.PostAsJsonAsync("messages", payload);
        response.EnsureSuccessStatusCode();
    }

    public async Task ReplyToMessage(WhatsAppMessage message, string body)
    {
        var payload = new
        {
            messaging_product = "whatsapp",
            recipient_type = "individual",
            to = message.PhoneNumber,
            context = new { message_id = message.WhatsAppId },
            type = "text",
            text = new { body }
        };

        using var response = await httpClient.PostAsJsonAsync("messages", payload);
        response.EnsureSuccessStatusCode();
    }

    public async Task ReactToMessage(WhatsAppMessage message, string emoji)
    {
        var payload = new
        {
            messaging_product = "whatsapp",
            recipient_type = "individual",
            to = message.PhoneNumber,
            type = "reaction",
            reaction = new { 
                message_id = message.WhatsAppId,
                emoji,
            },
        };

        using var response = await httpClient.PostAsJsonAsync(
            "messages",
            payload);
        response.EnsureSuccessStatusCode();
    }

    public async Task<WhatsAppMessageLinkResponseDto?> CreateMessageLink(string prefilledMessage)
    {
        var payload = new
        {
            prefilled_message = prefilledMessage,
            generate_qr_image = "PNG"
        };

        using var response = await httpClient.PostAsJsonAsync("message_qrdls", payload);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<WhatsAppMessageLinkResponseDto>();
    }
}
