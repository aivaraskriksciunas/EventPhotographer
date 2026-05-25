using EventPhotographer.Core.Features.Content.Entities;
using EventPhotographer.Core.Features.Users.Entities;
using EventPhotographer.Core.Util;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPhotographer.Core.Features.Events.Entities;

[EntityTypeConfiguration(typeof(UUIDEntityConfiguration<Event>))]
[Index(nameof(AdministratorAccessKey), IsUnique = true)]
public class Event : IEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? UserId { get; set; }

    [Required]
    public User? User { get; set; } = null;

    [Column(TypeName = "char")]
    [StringLength(36)]
    public string? AdministratorAccessKey { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<EventShareableLink> ShareableLinks { get; set; } = new List<EventShareableLink>();

    public ICollection<Participant> Participants { get; set; } = new List<Participant>();

    public ICollection<Media> Media { get; set; } = new List<Media>();
}