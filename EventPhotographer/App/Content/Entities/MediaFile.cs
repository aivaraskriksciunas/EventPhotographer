using EventPhotographer.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPhotographer.App.Content.Entities;

[EntityTypeConfiguration(typeof(UUIDEntityConfiguration<MediaFile>))]
public class MediaFile : IEntity
{
    public Guid Id { get; set; }

    [StringLength(255)]
    public required string Path { get; set; }

    [StringLength(255)]
    public required string MimeType { get; set; }

    [Column(TypeName = "int")]
    public required ulong FileSize { get; set; }

    public Guid MediaId { get; set; }
    public required Media Media { get; set; }
}
