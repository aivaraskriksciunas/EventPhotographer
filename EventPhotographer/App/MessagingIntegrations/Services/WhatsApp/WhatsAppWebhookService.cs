using EventPhotographer.Core.Configuration;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace EventPhotographer.App.MessagingIntegrations.Services.WhatsApp;

public class WhatsAppWebhookService(
    IOptions<WhatsAppConfiguration> _whatsappOptions)
{
    private readonly WhatsAppConfiguration configuration = _whatsappOptions.Value;

    public bool ValidateSignature(string signature, string body)
    {
        using HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(configuration.SecretKey));
        string hash = Convert.ToHexString(
            hmac.ComputeHash(Encoding.UTF8.GetBytes(body)))
            .ToUpper();

        return signature.ToUpper() == $"SHA256={hash}";
    }

    public bool IsVerificationTokenValid(string token)
    {
        return token == configuration.VerificationToken;
    }
}