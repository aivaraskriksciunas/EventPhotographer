using EventPhotographer.App.Users.Entities;
using EventPhotographer.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPhotographer.App.Events.Entities;

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
}