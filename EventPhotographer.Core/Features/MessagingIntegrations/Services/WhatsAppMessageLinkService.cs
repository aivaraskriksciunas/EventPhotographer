using EventPhotographer.Core.Features.Events.Entities;
using EventPhotographer.Core.Features.MessagingIntegrations.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventPhotographer.Core.Features.MessagingIntegrations.Services;

public class WhatsAppMessageLinkService(
    AppDbContext Db)
{
    public async Task<WhatsAppMessageLink> CreatePendingAsync(EventShareableLink shareableLink)
    {
        var @event = shareableLink.Event;
        if (@event == null)
        {
            @event = await Db.Events.FirstOrDefaultAsync(e => e.Id == shareableLink.EventId);
        }

        if (@event == null)
        {
            throw new ArgumentNullException($"Event does not exist for shareable link {shareableLink.Id}");
        }

        var link = new WhatsAppMessageLink
        {
            ShareableLink = shareableLink,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = @event.EndDate.AddDays(1),
        };

        await Db.WhatsAppMessageLinks.AddAsync(link);
        await Db.SaveChangesAsync();

        return link;
    }

    public async Task<WhatsAppMessageLink?> GetByIdAsync(Guid id)
    {
        return await Db.WhatsAppMessageLinks
            .Where(l => l.Id == id)
            .Include(l => l.ShareableLink)
            .FirstOrDefaultAsync();
    }

    public async Task<WhatsAppMessageLink> UpdateAsync(WhatsAppMessageLink link)
    {
        await Db.SaveChangesAsync();

        return link;
    }
}
