using EventPhotographer.Core.Features.MessagingIntegrations.Entities;

namespace EventPhotographer.Core.Features.MessagingIntegrations.Services;

public class WhatsAppMediaService(
    AppDbContext db)
{
    public async Task<WhatsAppMedia> AddAsync(WhatsAppMedia media)
    {
        await db.AddAsync(media);
        await db.SaveChangesAsync();

        return media;
    }
}
