using EventPhotographer.Core.Features.MessagingIntegrations.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventPhotographer.Core.Features.MessagingIntegrations.Services;

public class WhatsAppContactService(
    AppDbContext db)
{
    public async Task<WhatsAppContact?> GetByWhatsAppIdAsync(string whatsAppId)
    {
        return await db.WhatsAppContacts
            .Where(w => w.WhatsAppId == whatsAppId)
            .FirstOrDefaultAsync();
    }

    public async Task<WhatsAppContact> UpdateOrCreateAsync(WhatsAppContact contact)
    {
        WhatsAppContact? existingContact = db.WhatsAppContacts
            .Where(c => c.WhatsAppId == contact.WhatsAppId)
            .FirstOrDefault();

        if (existingContact == null)
        {
            await db.WhatsAppContacts.AddAsync(contact);
            await db.SaveChangesAsync();

            return contact;
        }

        if (existingContact.ProfileName != contact.ProfileName)
        {
            existingContact.ProfileName = contact.ProfileName;
            await db.SaveChangesAsync();
        }

        return existingContact;
    }

    public async Task<WhatsAppContact> UpdateAsync(WhatsAppContact contact)
    {
        await db.SaveChangesAsync();

        return contact;
    }
}
