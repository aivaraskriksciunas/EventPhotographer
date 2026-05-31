namespace EventPhotographer.App.MessagingIntegrations.DTO.Response;

public class WhatsAppMessageLinkResponseDto
{
    public Guid Id { get; set; }

    public string DeepLinkUrl { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }
}
