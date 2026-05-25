using EventPhotographer.Core.Features.Events.Entities;
using EventPhotographer.Core.Util;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EventPhotographer.Core.Features.MessagingIntegrations.Entities;

[EntityTypeConfiguration(typeof(UUIDEntityConfiguration<WhatsAppContact>))]
[Index(nameof(WhatsAppId), IsUnique = true)]
[Index(nameof(WhatsAppUserId), IsUnique = true)]
public class WhatsAppContact : IEntity
{
    public Guid Id { get; set; }

    [MaxLength(100)]
    public required string WhatsAppId { get; set; }

    /// <remarks>
    /// The WhatsApp documentation does not explicitly mention this parameter, 
    /// so we should not rely on it existing all the time.
    /// </remarks>
    [MaxLength(100)]
    public string? WhatsAppUserId { get; set; }

    [MaxLength(100)]
    public required string ProfileName { get; set; }

    public Participant? ActiveParticipant { get; set; } = null;
    public Guid? ActiveParticipantId { get; set; } = null;
}
