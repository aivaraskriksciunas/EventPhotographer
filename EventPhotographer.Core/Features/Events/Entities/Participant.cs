using EventPhotographer.Core.Features.Users.Entities;
using EventPhotographer.Core.Util;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EventPhotographer.Core.Features.Events.Entities;

[EntityTypeConfiguration(typeof(ParticipantEntityConfiguration))]
[Index(nameof(Token), IsUnique = true)]
public class Participant : IEntity
{
    public Guid Id { get; set; }

    public Guid Token { get; set; }

    [StringLength(100)]
    public required string Name { get; set; }

    public Guid EventId { get; set; }
    public required Event Event { get; set; }

    public string? UserId { get; set; } = null;
    public User? User { get; set; } = null;

    public Guid? EventShareableLinkId { get; set; } = null;
    public EventShareableLink? EventShareableLink { get; set; } = null;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
