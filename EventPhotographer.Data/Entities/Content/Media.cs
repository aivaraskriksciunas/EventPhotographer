using EventPhotographer.Data.Util;
using EventPhotographer.Data.Entities.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventPhotographer.Data.Entities.Content;

[EntityTypeConfiguration(typeof(MediaEntityConfiguration))]
public class Media : IEntity
{
    public Guid Id { get; set; }

    public Guid? UploadToken { get; set; } = null;

    public Guid EventId { get; set; }
    public required Event Event { get; set; }

    public Guid? ParticipantId { get; set; }
    public Participant? Participant { get; set; } = null;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<MediaFile> Files { get; set; } = new List<MediaFile>();
}

internal class MediaEntityConfiguration : UUIDEntityConfiguration<Media>
{
    public new void Configure(EntityTypeBuilder<Media> builder)
    {
        base.Configure(builder);

        builder.HasIndex(e => e.UploadToken);
        builder.Property(e => e.UploadToken)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("uuidv4()");
    }
}
