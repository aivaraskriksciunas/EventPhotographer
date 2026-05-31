using EventPhotographer.Core.Features.Events.Entities;
using EventPhotographer.Core.Features.MessagingIntegrations.Enums;
using EventPhotographer.Core.Util;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPhotographer.Core.Features.MessagingIntegrations.Entities;

[EntityTypeConfiguration(typeof(UUIDEntityConfiguration<WhatsAppMessageLink>))]
[Index(nameof(Code), IsUnique = true)]
public class WhatsAppMessageLink : IEntity
{
    public Guid Id { get; set; }

    public WhatsAppMessageLinkStatus Status { get; set; } = WhatsAppMessageLinkStatus.Pending;

    [MaxLength(100)]
    public string? Code { get; set; }

    public string? DeepLinkUrl { get; set; }

    [Column(TypeName = "text")]
    public string? PrefilledMessage { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public required DateTime ExpiresAt { get; set; }

    public required EventShareableLink ShareableLink { get; set; }
    public Guid ShareableLinkId { get; set; }
}
