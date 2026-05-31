using System.Text.Json.Serialization;

namespace EventPhotographer.Worker.Services.MessagingIntegrations.DTO;

internal class WhatsAppMessageLinkResponseDto
{
    [JsonPropertyName("code")]
    public required string Code { get; set; }

    [JsonPropertyName("prefilled_message")]
    public required string PrefilledMessage { get; set; }

    [JsonPropertyName("deep_link_url")]
    public required string DeepLinkUrl { get; set; }
}
