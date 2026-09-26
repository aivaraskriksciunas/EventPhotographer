using EventPhotographer.Core.Util;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EventPhotographer.Core.Features.Users.Entities;

[EntityTypeConfiguration(typeof(UUIDEntityConfiguration<AccountVerification>))]
[Index(nameof(Code), IsUnique = true)]
public class AccountVerification : IEntity
{
    public Guid Id { get; set; }

    [StringLength(100)]
    public required string Code { get; set; }

    public string UserId { get; set; } = string.Empty;
    public required User User { get; set; }

    public required DateTime CreatedAt { get; set; }
    public DateTime? ValidatedAt { get; set; } = null;
}
