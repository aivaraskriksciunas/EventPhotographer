using EventPhotographer.Core.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPhotographer.Core.Features.Content.Entities;

[EntityTypeConfiguration(typeof(MediaFileEntityConfiguration))]
public class MediaFile : IEntity
{
    public Guid Id { get; set; }

    [StringLength(255)]
    public required string Path { get; set; }

    [StringLength(255)]
    public required string MimeType { get; set; }

    [Column(TypeName = "int")]
    public required ulong FileSize { get; set; }

    public MediaFileType FileType { get; set; } = MediaFileType.Original;

    public Guid MediaId { get; set; }
    public required Media Media { get; set; }
}

internal class MediaFileEntityConfiguration : UUIDEntityConfiguration<MediaFile>
{
    public override void Configure(EntityTypeBuilder<MediaFile> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.FileType)
            .HasConversion<string>()
            .HasMaxLength(25)
            .HasDefaultValue(MediaFileType.Original);
    }
}