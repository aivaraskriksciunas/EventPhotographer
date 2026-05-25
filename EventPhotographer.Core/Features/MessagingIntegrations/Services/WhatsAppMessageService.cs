using EventPhotographer.Core.Features.MessagingIntegrations.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventPhotographer.Core.Features.MessagingIntegrations.Services;

public class WhatsAppMessageService(
    AppDbContext db)
{
    public async Task<WhatsAppMessage> CreateAsync(WhatsAppMessage message)
    {
        await db.WhatsAppMessages.AddAsync(message);
        await db.SaveChangesAsync();

        return message;
    }

    public async Task<bool> MessageIdExistsAsync(string whatsAppId)
    {
        return await db.WhatsAppMessages
            .Where(m => m.WhatsAppId == whatsAppId)
            .AnyAsync();
    }
}
