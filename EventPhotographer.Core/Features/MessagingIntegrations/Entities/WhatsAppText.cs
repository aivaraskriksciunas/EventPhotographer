using EventPhotographer.Core.Util;
using Microsoft.EntityFrameworkCore;
namespace EventPhotographer.Core.Features.MessagingIntegrations.Entities;

[EntityTypeConfiguration(typeof(UUIDEntityConfiguration<WhatsAppText>))]
public class WhatsAppText : IEntity
{
    public Guid Id { get; set; }

    public required string Body { get; set; }

    public required WhatsAppMessage WhatsAppMessage { get; set; }

    public Guid WhatsAppMessageId { get; set; }
}
