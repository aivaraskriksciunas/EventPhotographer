using EventPhotographer.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventPhotographer.App.Content.Entities;

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
