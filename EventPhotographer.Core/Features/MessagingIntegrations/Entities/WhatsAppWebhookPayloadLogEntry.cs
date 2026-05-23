using EventPhotographer.Core.Util;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPhotographer.Core.Features.MessagingIntegrations.Entities;

[EntityTypeConfiguration(typeof(UUIDEntityConfiguration<WhatsAppWebhookPayloadLogEntry>))]
[Index(nameof(Hash), IsUnique = true)]
public class WhatsAppWebhookPayloadLogEntry : IEntity
{
    public Guid Id { get; set; }

    [Column(TypeName = "text")]
    public required string Payload { get; set; }

    [MaxLength(100)]
    public required string Hash { get; set; }

    public bool IsValid { get; set; } = false;

    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
}
