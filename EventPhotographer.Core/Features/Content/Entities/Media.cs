using EventPhotographer.Core.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EventPhotographer.Core.Features.Events.Entities;

namespace EventPhotographer.Core.Features.Content.Entities;

[EntityTypeConfiguration(typeof(MediaEntityConfiguration))]
public class Media : IEntity
{
    public Guid Id { get; set; }

    public Guid? UploadToken { get; set; } = null;

    public MediaType Type { get; set; } = MediaType.Image;

    public Guid EventId { get; set; }
    public required Event Event { get; set; }

    public Guid? ParticipantId { get; set; }
    public Participant? Participant { get; set; } = null;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<MediaFile> Files { get; set; } = new List<MediaFile>();
}

internal class MediaEntityConfiguration : UUIDEntityConfiguration<Media>
{
    public override void Configure(EntityTypeBuilder<Media> builder)
    {
        base.Configure(builder);

        builder.HasIndex(e => e.UploadToken);
        builder.Property(e => e.UploadToken)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("uuidv4()");

        builder.Property(e => e.Type)
            .HasConversion<string>()
            .HasMaxLength(25);
    }
}
