using EventPhotographer.Core.Util;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EventPhotographer.Core.Features.MessagingIntegrations.Entities;

[EntityTypeConfiguration(typeof(UUIDEntityConfiguration<WhatsAppMedia>))]
[Index(nameof(WhatsAppId), IsUnique = true)]
public class WhatsAppMedia : IEntity
{
    public Guid Id { get; set; }

    [MaxLength(50)]
    public required string WhatsAppId { get; set; }

    [MaxLength(255)]
    public required string MimeType { get; set; }

    [MaxLength(255)]
    public string? Url { get; set; } = null;

    [MaxLength(100)]
    public string? Caption { get; set; } = null;

    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

    public required WhatsAppMessage WhatsAppMessage { get; set; }
    public Guid WhatsAppMessageId { get; set; }
}
