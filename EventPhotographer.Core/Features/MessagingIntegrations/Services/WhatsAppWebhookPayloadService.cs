using EventPhotographer.Core.Features.MessagingIntegrations.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace EventPhotographer.Core.Features.MessagingIntegrations.Services;

public class WhatsAppWebhookPayloadService(
    AppDbContext db)
{

    public async Task<WhatsAppWebhookPayload?> GetAsync(Guid id)
    {
        return await db.WhatsAppWebhookPayloadLogEntries
            .Where(w => w.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> WasWebhookAlreadyReceivedAsync(string hash)
    {
        return await db.WhatsAppWebhookPayloadLogEntries
            .Where(w => w.Hash == NormalizeHash(hash))
            .AnyAsync();
    }

    public async Task<WhatsAppWebhookPayload> LogWebhookPayloadAsync(string payload, string hash, bool isValid)
    {
        WhatsAppWebhookPayload entity = new WhatsAppWebhookPayload
        {
            Payload = payload,
            Hash = NormalizeHash(hash),
            IsValid = isValid,
            ReceivedAt = DateTime.UtcNow
        };

        await db.AddAsync(entity);
        await db.SaveChangesAsync();

        return entity;
    }

    private string NormalizeHash(string hash)
    {
        return hash.ToUpper();
    }
}
