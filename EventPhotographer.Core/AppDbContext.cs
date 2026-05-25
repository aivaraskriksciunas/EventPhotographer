using EventPhotographer.Core.Entities.AccountPolicies;
using EventPhotographer.Core.Features.Content.Entities;
using EventPhotographer.Core.Features.Events.Entities;
using EventPhotographer.Core.Features.MessagingIntegrations.Entities;
using EventPhotographer.Core.Features.Users.Entities;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EventPhotographer.Core;

public class AppDbContext : IdentityDbContext<User>, IDataProtectionKeyContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Event> Events { get; set; }

    public DbSet<AccountTier> AccountTiers { get; set; }

    public DbSet<EventShareableLink> EventShareableLinks { get; set; }

    public DbSet<Participant> Participants { get; set; }

    public DbSet<Media> Media { get; set; }

    public DbSet<MediaFile> MediaFiles { get; set; }

    public DbSet<WhatsAppWebhookPayload> WhatsAppWebhookPayloadLogEntries { get; set; }

    public DbSet<WhatsAppContact> WhatsAppContacts { get; set; }

    public DbSet<WhatsAppMessage> WhatsAppMessages { get; set; }

    public DbSet<WhatsAppText> WhatsAppTexts { get; set; }

    public DbSet<WhatsAppMedia> WhatsAppMedia { get; set; }

    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder
            .Properties<DateTime>()
            .HaveConversion<UtcDateTimeConverter>();
    }
}

internal class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
{
    public UtcDateTimeConverter() : base(
        v => v.ToUniversalTime(),
        v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
    { }
}
