using EventPhotographer.Core.Util;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EventPhotographer.Core.Features.MessagingIntegrations.Entities;

[EntityTypeConfiguration(typeof(UUIDEntityConfiguration<WhatsAppMessage>))]
[Index(nameof(WhatsAppId), IsUnique = true)]
public class WhatsAppMessage : IEntity
{
    public Guid Id { get; set; }

    public required string WhatsAppId { get; set; }

    [MaxLength(50)]
    public required string Type { get; set; }

    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

    public required WhatsAppContact WhatsAppContact { get; set; }
    public Guid WhatsAppContactId { get; set; }
}
