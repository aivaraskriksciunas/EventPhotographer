using EventPhotographer.Core.Features.MessagingIntegrations.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventPhotographer.Core.Features.MessagingIntegrations.Services;

public class WhatsAppWebhookPayloadLogService(
    AppDbContext db)
{
    public async Task<bool> WasWebhookAlreadyReceivedAsync(string hash)
    {
        return await db.WhatsAppWebhookPayloadLogEntries
            .Where(w => w.Hash == NormalizeHash(hash))
            .AnyAsync();
    }

    public async Task<WhatsAppWebhookPayloadLogEntry> LogWebhookPayload(string payload, string hash, bool isValid)
    {
        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        var entity = new WhatsAppWebhookPayloadLogEntry 
        { 
            Payload = Convert.ToBase64String(payloadBytes), 
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
