using EventPhotographer.Core.Features.MessagingIntegrations.Entities;
using EventPhotographer.Worker.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace EventPhotographer.Worker.Services.MessagingIntegrations.WhatsApp;

internal class WhatsAppClient(
    HttpClient httpClient,
    IOptions<WhatsAppConfiguration> _config)
{
    private readonly WhatsAppConfiguration config = _config.Value;

    public async Task MarkAsReadAsync(WhatsAppMessage message)
    {
        var payload = new
        {
            messaging_product = "whatsapp",
            status = "read",
            message_id = message.WhatsAppId,
        };

        using var response = await httpClient.PostAsync(
            "messages",
            JsonContent.Create(payload));
        response.EnsureSuccessStatusCode();
    }

    public async Task ReplyToMessage(WhatsAppMessage message, string body)
    {
        var payload = new Dictionary<string, object?>
        {
            ["messaging_product"] = "whatsapp",
            ["recipient_type"] = "individual",
            ["type"] = "text",
            ["text"] = new { body },
            ["context"] = new { message_id = message.WhatsAppId },
        };

        if (message.WhatsAppContact.WhatsAppUserId != null)
        {
            payload["recipient"] = message.WhatsAppContact.WhatsAppUserId;
        }
        else
        {
            payload["to"] = message.PhoneNumber;
        }

        using var response = await httpClient.PostAsync(
            "messages",
            JsonContent.Create(payload));
        response.EnsureSuccessStatusCode();
    }
}
