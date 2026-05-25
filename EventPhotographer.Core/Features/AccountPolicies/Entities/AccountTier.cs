using EventPhotographer.Core.Features.Users.Entities;
using EventPhotographer.Core.Util;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EventPhotographer.Core.Entities.AccountPolicies;

[EntityTypeConfiguration(typeof(UUIDEntityConfiguration<AccountTier>))]
public class AccountTier : IEntity
{
    public Guid Id { get; set; }

    public required User User { get; set; }

    public required AccountTierType Type { get; set; } = AccountTierType.FreeTier;

    [Required]
    public required DateTime From { get; set; }

    public DateTime? To { get; set; } = null;
}
