namespace EventPhotographer.Worker.Configuration;

internal class WhatsAppConfiguration
{
    /// <summary>
    /// Not to be confused with secret key
    /// </summary>
    public required string AccessToken { get; set; }

    public required string ApiVersion { get; set; }

    public required string BusinessPhoneNumberId { get; set; }
}
