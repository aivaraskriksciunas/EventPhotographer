using EventPhotographer.App.Events.Entities;
using EventPhotographer.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventPhotographer.App.Content.Entities;

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
