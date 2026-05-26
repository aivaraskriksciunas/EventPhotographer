using EventPhotographer.Core.Features.MessagingIntegrations.Entities;

namespace EventPhotographer.Core.Features.MessagingIntegrations.Services;

public class WhatsAppTextService(
    AppDbContext db)
{
    public async Task<WhatsAppText> CreateAsync(WhatsAppMessage message, string body)
    {
        var entity = new WhatsAppText
        {
            WhatsAppMessage = message,
            Body = body
        };

        await db.WhatsAppTexts.AddAsync(entity);
        await db.SaveChangesAsync();

        return entity;
    }
}
