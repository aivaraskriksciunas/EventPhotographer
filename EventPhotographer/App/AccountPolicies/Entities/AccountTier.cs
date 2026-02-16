using EventPhotographer.App.AccountPolicies.Resources;
using EventPhotographer.App.Events.Entities;
using EventPhotographer.App.Users.Entities;
using EventPhotographer.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EventPhotographer.App.AccountPolicies.Entities;

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
