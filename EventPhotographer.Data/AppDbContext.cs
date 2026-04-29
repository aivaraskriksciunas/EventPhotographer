using EventPhotographer.Data.Entities.Users;
using EventPhotographer.Data.Entities.Events;
using EventPhotographer.Data.Entities.AccountPolicies;
using EventPhotographer.Data.Entities.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EventPhotographer.Data;

public class AppDbContext : IdentityDbContext<User>
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
