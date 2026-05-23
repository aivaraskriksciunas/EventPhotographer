using EventPhotographer.Core;
using EventPhotographer.Core.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace EventPhotographer.App.MessagingIntegrations.Services;

public class WhatsAppWebhookService(
    IOptions<WhatsAppConfiguration> _whatsappOptions)
{
    private readonly WhatsAppConfiguration configuration = _whatsappOptions.Value;

    public bool ValidateSignature(string signature, string body)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(configuration.SecretKey));
        var hash = Convert.ToHexString(
            hmac.ComputeHash(Encoding.UTF8.GetBytes(body)))
            .ToUpper();

        return signature.ToUpper() == $"SHA256={hash}";
    }
}
