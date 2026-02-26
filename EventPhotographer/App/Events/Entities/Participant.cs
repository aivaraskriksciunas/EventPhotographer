using EventPhotographer.App.Users.Entities;
using EventPhotographer.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EventPhotographer.App.Events.Entities;

[EntityTypeConfiguration(typeof(UUIDEntityConfiguration<Participant>))]
[Index(nameof(Token), IsUnique = true)]
public class Participant : IEntity
{
    public Guid Id { get; set; }

    [StringLength(100)]
    public string Token = string.Empty;

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
