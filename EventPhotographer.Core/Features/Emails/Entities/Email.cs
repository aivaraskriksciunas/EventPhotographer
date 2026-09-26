using EventPhotographer.Core.Features.Content.Entities;
using EventPhotographer.Core.Features.Users.Entities;
using EventPhotographer.Core.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPhotographer.Core.Features.Emails.Entities;

[EntityTypeConfiguration(typeof(EmailTypeConfiguration))]
[Index(nameof(EmailStatus))]
public class Email : IEntity
{
    public Guid Id { get; set; }

    public required string Recipient { get; set; }

    public string? RecipientName { get; set; } = null;

    public string? UserId { get; set; } = null;
    public User? User { get; set; } = null;

    [MaxLength(255)]
    public required string Subject { get; set; }

    [Column(TypeName = "text")]
    public required string Body { get; set; }

    public EmailType? EmailType { get; set; } = null;

    public EmailStatus EmailStatus { get; set; } = EmailStatus.SCHEDULED;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? SentAt { get; set; } = null;
}

internal class EmailTypeConfiguration : UUIDEntityConfiguration<Email>
{
    public override void Configure(EntityTypeBuilder<Email> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.EmailStatus)
            .HasConversion<string>()
            .HasDefaultValue(EmailStatus.SCHEDULED)
            .HasMaxLength(25);

        builder.Property(e => e.EmailType)
            .HasConversion<string>()
            .HasMaxLength(100);
    }
}