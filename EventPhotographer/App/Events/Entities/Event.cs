using EventPhotographer.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventPhotographer.App.Events.Entities;

[EntityTypeConfiguration(typeof(UUIDEntityConfiguration<Event>))]
public class Event : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

//internal class EventConfiguration
//{

//}