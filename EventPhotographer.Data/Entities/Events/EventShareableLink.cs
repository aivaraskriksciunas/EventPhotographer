using EventPhotographer.Data.Util;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EventPhotographer.Data.Entities.Events;

[EntityTypeConfiguration(typeof(UUIDEntityConfiguration<EventShareableLink>))]
[Index(nameof(Code), IsUnique = true)]
public class EventShareableLink : IEntity
{
    public Guid Id { get; set; }

    [MaxLength(10)]
    public string Code { get; set; } = string.Empty;

    public Guid EventId { get; set; }

    [Required]
    public required Event Event { get; set; }
}
